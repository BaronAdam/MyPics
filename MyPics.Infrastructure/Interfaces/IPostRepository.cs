using System.Collections.Generic;
using System.Threading.Tasks;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> GetById(int postId);
        Task<Post> AddPost(Post post);
        Task<bool> EditPost(PostForUpdateDto post, int userId);
        Task<bool> DeletePost(int postId, int userId);
        Task<PostDto> GetPostForUser(int userId, int postId);
        Task<PagedList<PostDto>> GetPostsForUser(int userId, PostParameters parameters);
        Task<PagedList<PostDto>> GetPostsForFeed(List<int> userIds, PostParameters parameters);
    }
}