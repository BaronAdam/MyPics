using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Persistence;

namespace MyPics.Infrastructure.Repositories
{
    public class PictureRepository : IPictureRepository
    {
        private readonly MyPicsDbContext _context;

        public PictureRepository(MyPicsDbContext context)
        {
            _context = context;
        }
        
        public async Task<bool> AddPicturesForPost(List<Picture> pictures)
        {
            try
            {
                await _context.Pictures.AddRangeAsync(pictures);
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