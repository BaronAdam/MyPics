using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Persistence;

namespace MyPics.Infrastructure.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly MyPicsDbContext _context;
        private readonly IMapper _mapper;

        public LikeRepository(MyPicsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<bool> AddPostLike(int postId, int userId)
        {
            var like = new PostLike
            {
                PostId = postId,
                UserId = userId
            };
            try
            {
                _context.PostLikes.Add(like);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AddCommentLike(int commentId, int userId)
        {
            var like = new CommentLike
            {
                CommentId = commentId,
                UserId = userId
            };
            try
            {
                _context.CommentLikes.Add(like);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<CommentLike> GetCommentLike(int userId, int commentId)
        {
            try
            {
                return await _context.CommentLikes
                    .Where(x => x.UserId == userId && x.CommentId == commentId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PostLike> GetPostLike(int userId, int postId)
        {
            try
            {
                return await _context.PostLikes
                    .Where(x => x.UserId == userId && x.PostId == postId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        public async Task<bool> RemoveLike<T>(T like) where T : class
        {
            try
            {
                _context.Remove(like);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<PagedList<PostLikeForListDto>> GetLikesForPost(int postId, LikeParameters parameters)
        {
            try
            {
                var likes = _context.PostLikes
                    .Where(x => x.PostId == postId)
                    .Include(x => x.User)
                    .OrderBy(x => x.UserId)
                    .ProjectTo<PostLikeForListDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<PostLikeForListDto>.CreateAsync(likes, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PagedList<CommentLikeForListDto>> GetLikesForComment(int commentId, LikeParameters parameters)
        {
            try
            {
                var likes = _context.CommentLikes
                    .Where(x => x.CommentId == commentId)
                    .Include(x => x.User)
                    .OrderBy(x => x.UserId)
                    .ProjectTo<CommentLikeForListDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<CommentLikeForListDto>.CreateAsync(likes, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}