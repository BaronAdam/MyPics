using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Persistence
{
    public class MyPicsDbContext : DbContext
    {
        private readonly IEncryptionProvider _provider;

        public MyPicsDbContext(DbContextOptions<MyPicsDbContext> options, IConfiguration configuration) : base(options)
        {
            var encryptionKey = Convert.FromBase64String(configuration.GetSection("AppSettings:EncryptionKey").Value);
            _provider = new AesProvider(encryptionKey);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(_provider);

            modelBuilder.Entity<Follow>()
                .HasKey(x => new {x.FollowingId, x.UserId});

            modelBuilder.Entity<Follow>()
                .HasOne(x => x.User)
                .WithMany(x => x.Following)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(x => x.Following)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostLike>()
                .HasKey(x => new {x.PostId, x.UserId});

            modelBuilder.Entity<CommentLike>()
                .HasKey(x => new {x.CommentId, x.UserId});

            modelBuilder.Entity<CommentLike>()
                .HasOne(x => x.Comment)
                .WithMany(x => x.Likes)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<PostLike>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Likes)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<CommentLike>()
                .HasOne(x => x.User)
                .WithMany(x => x.CommentLikes)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<PostLike>()
                .HasOne(x => x.User)
                .WithMany(x => x.PostLikes)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserConversation>()
                .HasKey(x => new {x.UserId, x.ConversationId});
        }
    }
}