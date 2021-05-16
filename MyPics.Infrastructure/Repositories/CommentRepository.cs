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
    public class CommentRepository : ICommentRepository
    {
        private readonly MyPicsDbContext _context;
        private readonly IMapper _mapper;

        public CommentRepository(MyPicsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<Comment> Add(Comment comment)
        {
            try
            {
                var result = _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return result.Entity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> Remove(int commentId)
        {
            var comment = await GetCommentById(commentId);

            if (comment == null) return false;
            
            comment.IsDeleted = true;
            comment.Content = "[Deleted]";

            return await Update(comment) != null;
        }

        public async Task<Comment> Update(CommentForEditDto commentDto)
        {
            var comment = await GetCommentById(commentDto.Id);
            
            if (comment == null) return null;
            
            comment.Content = commentDto.Content;
            
            return await Update(comment);
        }

        public async Task<PagedList<CommentForListDto>> GetCommentsForPost(int postId, CommentParameters parameters)
        {
            try
            {
                var comments = _context.Comments
                    .Where(x => x.PostId == postId && !x.IsReply)
                    .OrderBy(x => x.DatePosted)
                    .Include(x => x.User)
                    .ProjectTo<CommentForListDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<CommentForListDto>.CreateAsync(comments, parameters.PageNumber,
                    parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PagedList<CommentForListDto>> GetRepliesForComment(int commentId, CommentParameters parameters)
        {
            try
            {
                var comments = _context.Comments
                    .Where(x => x.ParentCommentId == commentId)
                    .OrderBy(x => x.DatePosted)
                    .Include(x => x.User)
                    .ProjectTo<CommentForListDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<CommentForListDto>.CreateAsync(comments, parameters.PageNumber,
                    parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<int> GetNumberOfLikesForComment(int commentId)
        {
            try
            {
                return await _context.CommentLikes.Where(x => x.CommentId == commentId).CountAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        private async Task<Comment> Update(Comment comment)
        {
            try
            {
                var update = _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                return update.Entity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private async Task<Comment> GetCommentById(int commentId)
        {
            try
            {
                return await _context.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        public async Task<Comment> GetById(int commentId)
        {
            try
            {
                return await _context.Comments
                    .Where(x => x.Id == commentId)
                    .Include(x => x.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}