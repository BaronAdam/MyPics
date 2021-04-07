using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPics.Domain.Models;
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
            
            _context = new MyPicsDbContext(options, configuration.Object);

            _context.Posts.Add(new Post
            {
                Id = 1,
            });
            _context.Posts.Add(new Post
            {
                Id = 2,
            });

            _context.Pictures.Add(new Picture
            {
                Id = 1,
                PostId = 1
            });
            
            _context.SaveChanges();
            
            _repository = new PostRepository(_context);
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
            _repository = new PostRepository(null);
            
            var result = await _repository.AddPost(new Post());

            result.Should().BeNull();
        }

        [Test]
        public async Task DeletePost_ExistingPostAndPictures_ReturnsTrue()
        {
            var result = await _repository.DeletePost(1);

            result.Should().BeTrue();
        }

        [Test]
        public async Task DeletePost_NotExistingPictures_ReturnsTrue()
        {
            var result = await _repository.DeletePost(2);

            result.Should().BeTrue();
        }

        [Test]
        public async Task DeletePost_Exception_ReturnsFalse()
        {
            _repository = new PostRepository(null);
            
            var result = await _repository.DeletePost(1);

            result.Should().BeFalse();
        }
    }
}