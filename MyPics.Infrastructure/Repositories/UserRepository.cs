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
    public class UserRepository : IUserRepository
    {
        private readonly MyPicsDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(MyPicsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<User> GetUserById(int id)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PagedList<UserForFollowDto>> GetUserFollows(int userId, UserParameters parameters)
        {
            try
            {
                var follows = _context.Follows.Where(x => x.UserId == userId)
                    .Select(x => x.Following)
                    .OrderBy(x => x.Id)
                    .ProjectTo<UserForFollowDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<UserForFollowDto>.CreateAsync(follows, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PagedList<UserForFollowDto>> GetUserFollowers(int userId, UserParameters parameters)
        {
            try
            {
                var follows = _context.Follows.Where(x => x.FollowingId == userId)
                    .Select(x => x.User)
                    .OrderBy(x => x.Id)
                    .ProjectTo<UserForFollowDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<UserForFollowDto>.CreateAsync(follows, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<UserForFollowDto> FindUserInFollows(int userId, string username)
        {
            try
            {
                var user = await _context.Follows.Where(x => x.UserId == userId)
                    .Select(x => x.Following)
                    .ProjectTo<UserForFollowDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == username);
            
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<UserForFollowDto> FindUserInFollowers(int userId, string username)
        {
            try
            {
                var user = await _context.Follows.Where(x => x.FollowingId == userId)
                    .Select(x => x.User)
                    .ProjectTo<UserForFollowDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == username);
            
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}