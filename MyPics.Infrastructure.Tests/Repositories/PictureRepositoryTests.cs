using System.Collections.Generic;
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
    public class PictureRepositoryTests
    {
        private MyPicsDbContext _context;
        private PictureRepository _repository;

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

            _repository = new PictureRepository(_context);
        }
        
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task AddPicturesForPost_CorrectPictures_ReturnsTrue()
        {
            var pictures = new List<Picture>
            {
                new Picture(),
                new Picture()
            };
            
            var result = await _repository.AddPicturesForPost(pictures);

            result.Should().BeTrue();
        }

        [Test]
        public async Task AddPicturesForPost_Exception_ReturnsTrue()
        {
            _repository = new PictureRepository(null);
            
            var pictures = new List<Picture>
            {
                new Picture(),
                new Picture()
            };
            
            var result = await _repository.AddPicturesForPost(pictures);

            result.Should().BeFalse();
        }
    }
}