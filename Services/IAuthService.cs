using GlobalChat.Models;

namespace GlobalChat.Services
{
    public interface IAuthService
    {
        Task<User> Register(string username, string password);
        Task<User> Login(string username, string password);
    }
}
