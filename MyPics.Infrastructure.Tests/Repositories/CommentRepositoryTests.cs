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
    public class CommentRepositoryTests
    {
        private MyPicsDbContext _context;
        private CommentRepository _repository;
        
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
            
            var mapper = new Mock<IMapper>();
            
            mapper.Setup(x => x.ConfigurationProvider)
                .Returns(
                    () => new MapperConfiguration(
                        cfg =>
                        {
                            cfg.CreateMap<User, UserForCommentDto>();
                            cfg.CreateMap<Comment, CommentForListDto>();
                        }));
            
            _context = new MyPicsDbContext(options, configuration.Object);
            
            Seed();

            _repository = new CommentRepository(_context, mapper.Object);
        }
        
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Add_Successful_ReturnsComment()
        {
            var result = await _repository.Add(new Comment
            {
                PostId = 1,
                UserId = 1,
            });

            result.Should().NotBeNull();
            result.Should().BeOfType<Comment>();
        }
        
        [Test]
        public async Task Add_Exception_ReturnsNull()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.Add(new Comment
            {
                PostId = 1,
                UserId = 1,
            });

            result.Should().BeNull();
        }

        [Test]
        public async Task Remove_ExistingComment_ReturnsTrue()
        {
            var result = await _repository.Remove(1);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task Remove_NotExistingComment_ReturnsFalse()
        {
            var result = await _repository.Remove(100);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task Remove_Exception_ReturnsFalse()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.Remove(1);

            result.Should().BeFalse();
        }

        [Test]
        public async Task Update_ExistingComment_ReturnsUpdated()
        {
            var result = await _repository.Update(new CommentForEditDto
            {
                Id = 1,
                Content = "testContent after update"
            });

            result.Should().NotBeNull();
            result.Should().BeOfType<Comment>();
            result.Content.Should().Be("testContent after update");
        }
        
        [Test]
        public async Task Update_NotExistingComment_ReturnsNull()
        {
            var result = await _repository.Update(new CommentForEditDto
            {
                Id = 100,
                Content = "testContent after update"
            });

            result.Should().BeNull();
        }
        
        [Test]
        public async Task Update_Exception_ReturnsNull()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.Update(new CommentForEditDto
            {
                Id = 1,
                Content = "testContent after update"
            });

            result.Should().BeNull();
        }

        [Test]
        public async Task GetCommentsForPost_ExistingPost_ReturnsPagedList()
        {
            var result = await _repository.GetCommentsForPost(1, new CommentParameters());

            result.Should().NotBeNullOrEmpty();
        }
        
        [Test]
        public async Task GetCommentsForPost_NotExistingPost_ReturnsEmptyPagedList()
        {
            var result = await _repository.GetCommentsForPost(100, new CommentParameters());

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetCommentsForPost_Exception_ReturnsNull()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetRepliesForComment_ExistingComment_ReturnsPagedList()
        {
            var result = await _repository.GetRepliesForComment(1, new CommentParameters());

            result.Should().NotBeNullOrEmpty();
        }
        
        [Test]
        public async Task GetRepliesForComment_NotExistingComment_ReturnsEmptyPagedList()
        {
            var result = await _repository.GetRepliesForComment(100, new CommentParameters());

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetRepliesForComment_Exception_ReturnsNull()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetById_ExistingComment_ReturnsComment()
        {
            var result = await _repository.GetById(1);
            
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }
        
        [Test]
        public async Task GetById_NotExistingComment_ReturnsNull()
        {
            var result = await _repository.GetById(100);
            
            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetById_Exception_ReturnsNull()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.GetById(1);
            
            result.Should().BeNull();
        }

        [Test]
        public async Task GetNumberOfLikesForComment_ExistingLikes_ReturnsExpected()
        {
            var result = await _repository.GetNumberOfLikesForComment(1);

            result.Should().BePositive();
        }
        
        [Test]
        public async Task GetNumberOfLikesForComment_NotExistingLikes_ReturnsExpected()
        {
            var result = await _repository.GetNumberOfLikesForComment(2);

            result.Should().Be(0);
        }
        
        [Test]
        public async Task GetNumberOfLikesForComment_Exception_ReturnsExpected()
        {
            _repository = new CommentRepository(null, null);
            
            var result = await _repository.GetNumberOfLikesForComment(2);

            result.Should().Be(-1);
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

            _context.Posts.Add(new Post
            {
                Id = 1,
                UserId = 1,
            });
            _context.Posts.Add(new Post
            {
                Id = 2,
                UserId = 2,
            });

            _context.Comments.Add(new Comment
            {
                Id = 1,
                PostId = 1,
                UserId = 1
            });
            _context.Comments.Add(new Comment
            {
                Id = 2,
                PostId = 1,
                UserId = 2
            });
            _context.Comments.Add(new Comment
            {
                Id = 3,
                PostId = 2,
                UserId = 1
            });
            _context.Comments.Add(new Comment
            {
                Id = 4,
                PostId = 2,
                UserId = 2
            });
            _context.Comments.Add(new Comment
            {
                Id = 5,
                PostId = 1,
                UserId = 1,
                ParentCommentId = 1
            });
            _context.Comments.Add(new Comment
            {
                Id = 6,
                PostId = 1,
                UserId = 2,
                ParentCommentId = 1
            });
            _context.Comments.Add(new Comment
            {
                Id = 7,
                PostId = 2,
                UserId = 1,
                ParentCommentId = 3
            });
            _context.Comments.Add(new Comment
            {
                Id = 8,
                PostId = 2,
                UserId = 2,
                ParentCommentId = 3
            });
            
            _context.CommentLikes.Add(new CommentLike
            {
                CommentId = 1,
                UserId = 1
            });
            _context.CommentLikes.Add(new CommentLike
            {
                CommentId = 1,
                UserId = 2
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