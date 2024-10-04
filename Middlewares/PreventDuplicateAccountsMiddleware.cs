using System.Collections.Concurrent;

namespace GlobalChat.Middlewares
{
    public class PreventDuplicateAccountsMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, bool> RegisteredUsers = new();

        public PreventDuplicateAccountsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post && context.Request.Path.StartsWithSegments("/api/auth/register"))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                if (ipAddress != null && RegisteredUsers.ContainsKey(ipAddress))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Данный IP-адрес уже зарегистрирован.");
                    return;
                }
            }

            await _next(context);
        }

        public static void RegisterUser(string ipAddress)
        {
            RegisteredUsers.TryAdd(ipAddress, true);
        }
    }
}
