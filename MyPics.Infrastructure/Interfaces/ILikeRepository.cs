using System.Threading.Tasks;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;

namespace MyPics.Infrastructure.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> AddPostLike(int postId, int userId);
        Task<bool> AddCommentLike(int commentId, int userId);
        Task<CommentLike> GetCommentLike(int userId, int commentId);
        Task<PostLike> GetPostLike(int userId, int postId);
        Task<bool> RemoveLike<T>(T like) where T : class;
        Task<PagedList<PostLikeForListDto>> GetLikesForPost(int postId, LikeParameters parameters);
        Task<PagedList<CommentLikeForListDto>> GetLikesForComment(int commentId, LikeParameters parameters);
    }
}