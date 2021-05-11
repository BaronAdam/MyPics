using System;
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
            SeedPosts();
            SeedComments();
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
                var users = JsonSerializer.Deserialize<IEnumerable<User>>(userData);
                var i = 0;
                foreach (var user in users)
                {
                    CreatePasswordHash("password", out var passwordHash, out var passwordSalt);

                    if (i % 2 == 0) user.IsPrivate = true;
                    
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.IsConfirmed = true;
                    _context.Users.Add(user);

                    i++;
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
        
        private void SeedPosts()
        {
            var rnd = new Random();
            if (!_context.Posts.Any())
            {
                for (var i = 1; i <= 5; i++)
                {
                    for (var j = 1; j <= 5; j++)
                    {
                        if (i == j) continue;
                        var post = new Post
                        {
                            UserId = i,
                            Description = "lorem ipsum",
                            DatePosted = DateTime.UtcNow.AddDays(-rnd.Next(0, 10))
                        };
                        _context.Posts.Add(post);
                    }
                }
                _context.SaveChanges();
            }
        }

        private void SeedComments()
        {
            var rnd = new Random();
            if (!_context.Comments.Any())
            {
                for (var i = 1; i <= 20; i++)
                {
                    _context.Comments.Add(new Comment
                    {
                        PostId = i,
                        UserId = rnd.Next(1, 6),
                        Content = $"Comment no. {i}",
                        DatePosted = DateTime.Now.AddMinutes(-rnd.Next(30, 61))
                    });
                }

                _context.SaveChanges();
                
                for (var i = 1; i <= 20; i++)
                {
                    for (var j = 1; j <= 5; j++)
                    {
                        _context.Comments.Add(new Comment
                        {
                            PostId = i,
                            ParentCommentId = i,
                            UserId = j,
                            IsReply = true,
                            Content = $"Reply no. {j}",
                            DatePosted = DateTime.Now.AddMinutes(-rnd.Next(0, 31))
                        });
                    }
                }
                
                _context.SaveChanges();
            }
        }
    }
}