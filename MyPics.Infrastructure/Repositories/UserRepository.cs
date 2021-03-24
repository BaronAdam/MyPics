using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPics.Domain.Models;
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

        public async Task<bool> ChangeProfilePicture(int id, string pictureUrl)
        {
            var user = await GetUserById(id);

            if (user == null) return false;

            user.ProfilePictureUrl = pictureUrl;

            return await UpdateUser(user);
        }

        private async Task<bool> UpdateUser(User user)
        {
            try
            {
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}