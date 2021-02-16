using System.Threading.Tasks;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByUsername(string username);
    }
}