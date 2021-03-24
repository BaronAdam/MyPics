using System.Threading.Tasks;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByUsername(string username);
        Task<bool> ChangeProfilePicture(int id, string pictureUrl);
    }
}