using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public PostRepository(MyPicsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<bool> EditPost(PostForUpdateDto postForUpdate, int userId)
        {
            var post = await GetPostById(postForUpdate.Id);
            try
            {
                if (userId != post.UserId) return false;
                _mapper.Map(postForUpdate, post);
                _context.Update(post);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> DeletePost(int postId, int userId)
        {
            var post = await GetPostById(postId);
            try
            {
                if (userId != post.UserId) return false;
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

        public async Task<PostDto> GetPostForUser(int userId, int postId)
        {
            try
            {
                return await _context.Posts.Where(x => x.Id == postId && x.UserId == userId)
                    .Include(x => x.User)
                    .AsNoTracking()
                    .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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