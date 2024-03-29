﻿using System;
using System.Collections.Generic;
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
    public class FollowRepository : IFollowRepository
    {
        private readonly MyPicsDbContext _context;
        private readonly IMapper _mapper;
        
        public FollowRepository(MyPicsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                var queryable = user.IsPrivate
                    ? _context.Follows.Where(x => x.FollowingId == userId && x.IsAccepted)
                    : _context.Follows.Where(x => x.FollowingId == userId);
                    
                var follows = queryable.Select(x => x.User)
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

        public async Task<bool> FollowUser(int userId, int followeeId)
        {
            var follow = new Follow
            {
                UserId = userId,
                FollowingId = followeeId,
                IsAccepted = false
            };
            
            try
            {
                _context.Follows.Add(follow);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<FollowStatusDto> GetFollowStatus(int userId, int followeeId)
        {
            var follow = await GetFollow(userId, followeeId);

            if (follow == null) return new FollowStatusDto();

            return follow.IsAccepted ? new FollowStatusDto(true, true) : new FollowStatusDto(true);
        }

        public async Task<bool> UnFollowUser(int userId, int followeeId)
        {
            var follow = await GetFollow(userId, followeeId);

            try
            {
                _context.Follows.Remove(follow);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AcceptFollow(int userId, int followerId)
        {
            var follow = await GetFollow(followerId, userId);

            if (follow == null || follow.IsAccepted) return false;

            follow.IsAccepted = true;
            
            try
            {
                _context.Follows.Update(follow);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> RejectFollow(int userId, int followerId)
        {
            var follow = await GetFollow(followerId, userId);

            if (follow == null || follow.IsAccepted) return false;
            
            try
            {
                _context.Follows.Remove(follow);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> RemoveFollower(int userId, int followerId)
        {
            var follow = await GetFollow(followerId, userId);

            if (follow == null) return false;
            
            try
            {
                _context.Follows.Remove(follow);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<PagedList<UserForFollowDto>> GetNotAcceptedFollows(UserParameters parameters, int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (!user.IsPrivate) return new PagedList<UserForFollowDto>(new List<UserForFollowDto>(), 0, 0, 0);
                
                var users = _context.Follows.Where(x => x.FollowingId == userId && x.IsAccepted == false)
                    .Select(x => x.User)
                    .OrderBy(x => x.Id)
                    .ProjectTo<UserForFollowDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking();

                return await PagedList<UserForFollowDto>.CreateAsync(users, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<IEnumerable<int>> GetAllAcceptedFollowIds(int userId)
        {
            try
            {
                var notAcceptedFollows = await _context.Follows
                    .Where(x => x.UserId == userId && !x.IsAccepted)
                    .Select(x => x.Following)
                    .Where(x => x.IsPrivate)
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .ToListAsync();

                var follows = await _context.Follows
                    .Where(x => x.UserId == userId)
                    .Select(x => x.Following)
                    .OrderBy(x => x.Id)
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .ToListAsync();

                return follows.Except(notAcceptedFollows);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private async Task<Follow> GetFollow(int userId, int followeeId)
        {
            try
            {
                return await _context.Follows
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.FollowingId == followeeId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}