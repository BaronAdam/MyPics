using System.Threading.Tasks;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> AddPost(Post post);
        Task<bool> EditPost(PostForUpdateDto post, int userId);
        Task<bool> DeletePost(int postId, int userId);
        Task<PostDto> GetPostForUser(int userId, int postId);
    }
}