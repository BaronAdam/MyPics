using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Persistence;
using MyPics.Infrastructure.Repositories;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class LikeRepositoryTests
    {
        private MyPicsDbContext _context;
        private LikeRepository _repository;

        [SetUp]
        public void Setup()
        {
            var mockKeySection = new Mock<IConfigurationSection>();
            mockKeySection.Setup(c => c.Value)
                .Returns("TAf30yv4g15177S6EW6idxfE5YxyJiCX8Wf2c4nf9Aw=");
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(() => mockKeySection.Object);
            
            var options = new DbContextOptionsBuilder<MyPicsDbContext>()
                .UseInMemoryDatabase("my_pics")
                .Options;
            
            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<PostLike, PostLikeForListDto>();
                c.CreateMap<CommentLike, CommentLikeForListDto>();
                c.CreateMap<User, UserForLikeDto>();
            });

            var mapper = config.CreateMapper();
            
            _context = new MyPicsDbContext(options, configuration.Object);

            Seed();
            
            _repository = new LikeRepository(_context, mapper);
        }
        
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task AddPostLike_ExistingPost_ReturnsTrue()
        {
            var result = await _repository.AddPostLike(5, 1);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task AddPostLike_ExistingLike_ReturnsFalse()
        {
            var result = await _repository.AddPostLike(1, 1);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task AddPostLike_Exception_ReturnsFalse()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.AddPostLike(5, 1);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task AddCommentLike_ExistingComment_ReturnsTrue()
        {
            var result = await _repository.AddCommentLike(5, 1);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task AddCommentLike_ExistingLike_ReturnsFalse()
        {
            var result = await _repository.AddCommentLike(1, 1);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task AddCommentLike_Exception_ReturnsFalse()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.AddCommentLike(5, 1);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetCommentLike_ExistingLike_ReturnsLike()
        {
            var result = await _repository.GetCommentLike(1, 1);

            result.Should().NotBeNull();
            result.CommentId.Should().Be(1);
            result.UserId.Should().Be(1);
        }

        [TestCase(1, 100)]
        [TestCase(100, 1)]
        [TestCase(100, 100)]
        public async Task GetCommentLike_NotExistingLike_ReturnsNull(int userId, int commentId)
        {
            var result = await _repository.GetCommentLike(userId, commentId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetCommentLike_Exception_ReturnsNull()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.GetCommentLike(1, 1);

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetPostLike_ExistingLike_ReturnsLike()
        {
            var result = await _repository.GetPostLike(1, 1);

            result.Should().NotBeNull();
            result.PostId.Should().Be(1);
            result.UserId.Should().Be(1);
        }

        [TestCase(1, 100)]
        [TestCase(100, 1)]
        [TestCase(100, 100)]
        public async Task GetPostLike_NotExistingLike_ReturnsNull(int userId, int postId)
        {
            var result = await _repository.GetPostLike(userId, postId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetPostLike_Exception_ReturnsNull()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.GetPostLike(1, 1);

            result.Should().BeNull();
        }

        [Test]
        public async Task RemoveLike_NotExistingLike_ReturnsTrue()
        {

            var result = await _repository.RemoveLike(new PostLike
            {
                PostId = 100,
                UserId = 2
            });

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task RemoveLike_Exception_ReturnsTrue()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.RemoveLike(new PostLike
            {
                PostId = 1,
                UserId = 2
            });

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetLikesForPost_ExistingLikes_ReturnsPagedList()
        {
            var result = await _repository.GetLikesForPost(1, new LikeParameters());

            result.Should().NotBeNullOrEmpty();
        }
        
        [Test]
        public async Task GetLikesForPost_NotExistingLikes_ReturnsEmptyPagedList()
        {
            var result = await _repository.GetLikesForPost(100, new LikeParameters());

            result.Should().BeEmpty();
        }

        [Test] public async Task GetLikesForPost_Exception_ReturnsNull()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.GetLikesForPost(1, new LikeParameters());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetLikesForComment_ExistingLikes_ReturnsPagedList()
        {
            var result = await _repository.GetLikesForComment(1, new LikeParameters());

            result.Should().NotBeNullOrEmpty();
        }
        
        [Test]
        public async Task GetLikesForComment_NotExistingLikes_ReturnsEmptyPagedList()
        {
            var result = await _repository.GetLikesForComment(100, new LikeParameters());

            result.Should().BeEmpty();
        }

        [Test] public async Task GetLikesForComment_Exception_ReturnsNull()
        {
            _repository = new LikeRepository(null, null);
            
            var result = await _repository.GetLikesForComment(1, new LikeParameters());

            result.Should().BeNull();
        }

        private void Seed()
        {
            _context.Users.Add(new User
            {
                Id = 1,
                Username = "testUsername1",
                Email = "test1@email.com",
                IsPrivate = true
            });
            _context.Users.Add(new User
            {
                Id = 2,
                Username = "testUsername2",
                Email = "test2@email.com",
            });
            _context.Users.Add(new User
            {
                Id = 3,
                Username = "testUsername3",
                Email = "test3@email.com",
            });

            var userId = 1;
            for (var i = 1; i <= 5; i++)
            {
                userId = userId > 3 ? 1 : userId;
                
                _context.Posts.Add(new Post
                {
                    Id = i,
                    UserId = userId
                });
                _context.Comments.Add(new Comment
                {
                    Id = i,
                    UserId = userId,
                    PostId = i
                });

                userId++;
            }
            
            _context.PostLikes.Add(new PostLike
            {
                UserId = 1,
                PostId = 1
            });
            _context.PostLikes.Add(new PostLike
            {
                UserId = 2,
                PostId = 1
            });
            _context.CommentLikes.Add(new CommentLike
            {
                UserId = 1,
                CommentId = 1
            });
            _context.CommentLikes.Add(new CommentLike
            {
                UserId = 2,
                CommentId = 1
            });
            
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}