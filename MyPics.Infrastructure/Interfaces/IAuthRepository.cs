using System.Threading.Tasks;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<bool> EmailExists(string email);
        Task<bool> ConfirmEmail(string token, string username);
    }
}