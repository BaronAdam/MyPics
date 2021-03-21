using System.Threading.Tasks;
using MyPics.Domain.DTOs;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IFollowRepository
    {
        Task<PagedList<UserForFollowDto>> GetUserFollows(int userId, UserParameters parameters);
        Task<PagedList<UserForFollowDto>> GetUserFollowers(int userId, UserParameters parameters);
        Task<UserForFollowDto> FindUserInFollows(int userId, string username);
        Task<UserForFollowDto> FindUserInFollowers(int userId, string username);
        Task<bool> FollowUser(int userId, int followeeId);
        Task<FollowStatusDto> GetFollowStatus(int userId, int followeeId);
        Task<bool> UnFollowUser(int userId, int followeeId);
        Task<bool> AcceptFollow(int userId, int followerId);
        Task<bool> RejectFollow(int userId, int followerId);
        Task<bool> RemoveFollower(int userId, int followerId);
        Task<PagedList<UserForFollowDto>> GetNotAcceptedFollows (UserParameters parameters, int userId);
    }
}