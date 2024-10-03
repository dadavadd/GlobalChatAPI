
using GlobalChat.Data;
using GlobalChat.Hubs;
using GlobalChat.Services;
using Microsoft.EntityFrameworkCore;

namespace GlobalChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql("Host=ep-ancient-cherry-a594nlcd.us-east-2.aws.neon.tech;Database=GlobalChat;Username=GlobalChat_owner;Password=eipE1HkFnyX3;SSL Mode=Require"));
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddSignalR();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowed(_ => true));
            });

            var app = builder.Build();

            app.UseCors("AllowAll");
            app.UseRouting();
            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
