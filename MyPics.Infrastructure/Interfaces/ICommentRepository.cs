using System.Threading.Tasks;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;

namespace MyPics.Infrastructure.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> GetById(int commentId);
        Task<Comment> Add(Comment comment);
        Task<bool> Remove(int commentId);
        Task<Comment> Update(CommentForEditDto commentDto);
        Task<PagedList<CommentForListDto>> GetCommentsForPost(int postId, CommentParameters parameters);
        Task<PagedList<CommentForListDto>> GetRepliesForComment(int commentId, CommentParameters parameters);
    }
}