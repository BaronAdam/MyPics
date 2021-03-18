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
        Task<PagedList<UserForFollowDto>> GetUserFollows(int userId, UserParameters parameters);
        Task<PagedList<UserForFollowDto>> GetUserFollowers(int userId, UserParameters parameters);
        Task<UserForFollowDto> FindUserInFollows(int userId, string username);
        Task<UserForFollowDto> FindUserInFollowers(int userId, string username);
    }
}