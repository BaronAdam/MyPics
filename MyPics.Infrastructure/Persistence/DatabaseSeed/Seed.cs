using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Persistence.DatabaseSeed
{
    public class Seed
    {
        private readonly MyPicsDbContext _context;
        public Seed(MyPicsDbContext context)
        {
            _context = context;
        }

        public void SeedData()
        {
            if (!_context.Users.Any())
            {
                var userData = File
                    .ReadAllText("../MyPics.Infrastructure/Persistence/DatabaseSeed/UserData.json");
                var users = JsonSerializer.Deserialize<List<User>>(userData);
                foreach (var user in users)
                {
                    CreatePasswordHash("password", out var passwordHash, out var passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    _context.Users.Add(user);
                }

                _context.SaveChanges();
            }
        }
        
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}