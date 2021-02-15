using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Persistence;

namespace MyPics.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyPicsDbContext _context;

        public UserRepository(MyPicsDbContext context)
        {
            _context = context;
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
    }
}