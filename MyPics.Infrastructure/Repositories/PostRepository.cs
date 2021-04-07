using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Persistence;

namespace MyPics.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly MyPicsDbContext _context;

        public PostRepository(MyPicsDbContext context)
        {
            _context = context;
        }
        
        public async Task<Post> AddPost(Post post)
        {
            Post result;
            
            try
            {
                result = _context.Posts.AddAsync(post).Result.Entity;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return result;
        }

        public async Task<bool> EditPost(PostForUpdateDto post)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> DeletePost(int postId)
        {
            try
            {
                var post = _context.Posts.FirstOrDefault(x => x.Id == postId);
                var pictures = _context.Pictures.Where(x => x.PostId == postId);

                _context.Posts.Remove(post);
                _context.Pictures.RemoveRange(pictures);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private async Task<Post> GetPostById(int postId)
        {
            try
            {
                return await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}