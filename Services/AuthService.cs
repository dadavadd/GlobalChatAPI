using GlobalChat.Data;
using GlobalChat.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalChat.Services
{
    public class AuthService : IAuthService
    {
        private readonly ChatDbContext _context;

        public AuthService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<User> Register(string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new Exception("Username already exists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Username = username, PasswordHash = passwordHash };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            return user;
        }
    }
}
