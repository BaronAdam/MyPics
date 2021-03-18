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
            SeedUsers();
            SeedFollows();
        }
        
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private void SeedUsers()
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
                    user.IsConfirmed = true;
                    _context.Users.Add(user);
                }
                _context.SaveChanges();
            }
        }

        private void SeedFollows()
        {
            if (!_context.Follows.Any())
            {
                for (var i = 1; i <= 5; i++)
                {
                    for (var j = 1; j <= 5; j++)
                    {
                        if (i == j) continue;
                        var follow = new Follow
                        {
                            UserId = i,
                            FollowingId = j
                        };
                        _context.Follows.Add(follow);
                    }
                }
                _context.SaveChanges();
            }
        }
    }
}