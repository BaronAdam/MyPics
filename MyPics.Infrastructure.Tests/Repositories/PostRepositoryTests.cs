using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Persistence;
using MyPics.Infrastructure.Repositories;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class PostRepositoryTests
    {
        private MyPicsDbContext _context;
        private PostRepository _repository;

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
                c.CreateMap<PostForUpdateDto, Post>();
                c.CreateMap<Post, PostDto>();
                c.CreateMap<User, UserForPostDto>();
                c.CreateMap<Picture, PictureForPostDto>();
            });

            var mapper = config.CreateMapper();
            
            _context = new MyPicsDbContext(options, configuration.Object);
            
            Seed();

            _repository = new PostRepository(_context, mapper);
        }
        
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task AddPost_CorrectPost_ReturnsPost()
        {
            var result = await _repository.AddPost(new Post());

            result.Should().NotBeNull();
            result.Should().BeOfType<Post>();
        }

        [Test]
        public async Task AddPost_NullUser_ReturnsNull()
        {
            var result = await _repository.AddPost(null);

            result.Should().BeNull();
        }
        
        [Test]
        public async Task AddPost_Exception_ReturnsNull()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.AddPost(new Post());

            result.Should().BeNull();
        }

        [Test]
        public async Task DeletePost_ExistingPostAndPictures_ReturnsTrue()
        {
            var result = await _repository.DeletePost(1, 1);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task DeletePost_ExistingPostWrongUser_ReturnsFalse()
        {
            var result = await _repository.DeletePost(1, 100);

            result.Should().BeFalse();
        }

        [Test]
        public async Task DeletePost_NotExistingPictures_ReturnsTrue()
        {
            var result = await _repository.DeletePost(2, 1);

            result.Should().BeTrue();
        }

        [Test]
        public async Task DeletePost_Exception_ReturnsFalse()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.DeletePost(1, 1);

            result.Should().BeFalse();
        }

        [Test]
        public async Task EditPost_ExistingPost_ReturnsTrue()
        {
            var dto = new PostForUpdateDto
            {
                Id = 1,
                Description = "testDescription"
            };

            var result = await _repository.EditPost(dto, 1);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task EditPost_ExistingPostWrongUser_ReturnsFalse()
        {
            var dto = new PostForUpdateDto
            {
                Id = 1,
                Description = "testDescription"
            };

            var result = await _repository.EditPost(dto, 100);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task EditPost_NotExistingPost_ReturnsFalse()
        {
            var dto = new PostForUpdateDto
            {
                Id = 1000,
                Description = "testDescription"
            };

            var result = await _repository.EditPost(dto, 1);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task EditPost_Exception_ReturnsFalse()
        {
            _repository = new PostRepository(null, null);
            
            var dto = new PostForUpdateDto
            {
                Id = 1,
                Description = "testDescription"
            };

            var result = await _repository.EditPost(dto, 1);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetPostForUser_ExistingPost_ReturnsPostDto()
        {
            var result = await _repository.GetPostForUser(1, 1);

            result.Should().NotBeNull();
            result.User.Id.Should().Be(1);
            result.Id.Should().Be(1);
        }
        
        [Test]
        public async Task GetPostForUser_NotExistingPost_ReturnsNull()
        {
            var result = await _repository.GetPostForUser(1, 100);

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetPostForUser_NotExistingUser_ReturnsNull()
        {
            var result = await _repository.GetPostForUser(1000, 1);

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetPostForUser_Exception_ReturnsNull()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.GetPostForUser(1000, 1);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetPostsForUser_ExistingPosts_ReturnsPagedList()
        {
            var result = await _repository.GetPostsForUser(1, new PostParameters
            {
                PageNumber = 1,
                PageSize = 10
            });

            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<PagedList<PostDto>>();
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
        }
        
        [Test]
        public async Task GetPostsForUser_NotExistingPosts_ReturnsPagedList()
        {
            var result = await _repository.GetPostsForUser(100, new PostParameters
            {
                PageNumber = 1,
                PageSize = 10
            });

            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetPostsForUser_Exception_ReturnsNull()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.GetPostsForUser(1, new PostParameters
            {
                PageNumber = 1,
                PageSize = 10
            });

            result.Should().BeNull();
        }

        [Test]
        public async Task GetPostsForFeed_ExistingPosts_ReturnsPagedList()
        {
            var result = await _repository.GetPostsForFeed(new List<int> {1, 2}, new PostParameters
            {
                PageNumber = 1,
                PageSize = 10
            });
            
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<PagedList<PostDto>>();
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Test]
        public async Task GetPostsForFeed_NotExistingPosts_ReturnsPagedList()
        {
            var result = await _repository.GetPostsForFeed(new List<int> {100, 200}, new PostParameters
            {
                PageNumber = 1,
                PageSize = 10
            });
            
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetPostsForFeed_Exception_ReturnsNull()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.GetPostsForFeed(new List<int> {1, 2}, new PostParameters
            {
                PageNumber = 1,
                PageSize = 10
            });
            
            result.Should().BeNull();
        }

        [Test]
        public async Task GetPostById_ExistingPost_ReturnsPost()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }
        
        [Test]
        public async Task GetPostById_NotExistingPost_ReturnsNull()
        {
            var result = await _repository.GetById(100);

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetPostById_Exception_ReturnsNull()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.GetById(1);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetNumberOfLikesForPost_ExistingLikes_ReturnsExpected()
        {
            var result = await _repository.GetNumberOfLikesForPost(1);

            result.Should().BePositive();
        }

        [Test]
        public async Task GetNumberOfLikesForPost_NotExistingLikes_ReturnsExpected()
        {
            var result = await _repository.GetNumberOfLikesForPost(2);

            result.Should().Be(0);
        }
        
        [Test]
        public async Task GetNumberOfLikesForPost_Exception_ReturnsExpected()
        {
            _repository = new PostRepository(null, null);
            
            var result = await _repository.GetNumberOfLikesForPost(1);

            result.Should().Be(-1);
        }

        private void Seed()
        {
            _context.Posts.Add(new Post
            {
                Id = 1,
                UserId = 1,
                DatePosted = new DateTime(2021, 1, 1)
            });
            _context.Posts.Add(new Post
            {
                Id = 2,
                UserId = 1,
                DatePosted = new DateTime(2021, 1, 1)
            });
            
            _context.Posts.Add(new Post
            {
                Id = 3,
                UserId = 2,
                DatePosted = new DateTime(2021, 1, 1)
            });
            
            _context.Pictures.Add(new Picture
            {
                Id = 1,
                PostId = 1
            });
            _context.Pictures.Add(new Picture
            {
                Id = 2,
                PostId = 2
            });
            _context.Pictures.Add(new Picture
            {
                Id = 3,
                PostId = 3
            });

            _context.Users.Add(new User
            {
                Id = 1
            });
            _context.Users.Add(new User
            {
                Id = 2
            });

            _context.PostLikes.Add(new PostLike
            {
                PostId = 1,
                UserId = 1
            });
            _context.PostLikes.Add(new PostLike
            {
                PostId = 1,
                UserId = 2
            });

            _context.Comments.Add(new Comment
            {
                PostId = 1,
                UserId = 1
            });

            _context.SaveChanges();
        }
    }
}