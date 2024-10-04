using GlobalChat.Data;
using GlobalChat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GlobalChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;

        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string username, string message)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                var newMessage = new Message
                {
                    Content = message,
                    Timestamp = DateTime.UtcNow,
                    UserId = user.Id
                };
                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                await Clients.All.SendAsync("ReceiveMessage", username, message);
            }
        }

        public async Task GetRecentMessages()
        {
            var messages = await _context.Messages
                .OrderBy(m => m.Timestamp)
                .Take(100)
                .ToListAsync();

            foreach (var message in messages)
            {
                var user = await _context.Users.FindAsync(message.UserId);
                if (user != null)
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", user.Username, message.Content);
                }
            }
        }
    }
}
