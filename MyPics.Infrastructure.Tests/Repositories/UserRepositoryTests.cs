using System;
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
    public class UserRepositoryTests
    {
        private MyPicsDbContext _context;
        private UserRepository _repository;

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
                .UseInMemoryDatabase(databaseName: "my_pics")
                .Options;
            
            _context = new MyPicsDbContext(options, configuration.Object);

            _context.Users.Add(new User
            {
                Id = 1,
                Username = "testUsername1",
                Email = "test1@email.com",
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

            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _repository = new UserRepository(_context);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetUserById_ExistingUser_ReturnsExpectedUser(int id)
        {
            var result = await _repository.GetUserById(id);

            result.Should().NotBeNull();
            result.Id.Should().Be(id);
        }
        
        [TestCase(100)]
        [TestCase(200)]
        [TestCase(300)]
        public async Task GetUserById_NotExistingUser_ReturnsNull(int id)
        {
            var result = await _repository.GetUserById(id);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetUserById_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null);
            
            var result = await _repository.GetUserById(1);

            result.Should().BeNull();
        }
        
        [TestCase("testUsername1")]
        [TestCase("testUsername2")]
        [TestCase("testUsername3")]
        public async Task GetUserByUsername_ExistingUser_ReturnsExpectedUser(string username)
        {
            var result = await _repository.GetUserByUsername(username);

            result.Should().NotBeNull();
            result.Username.Should().Be(username);
        }
        
        [TestCase("notExistingTestUsername1")]
        [TestCase("notExistingTestUsername1")]
        [TestCase("notExistingTestUsername1")]
        public async Task GetUserByUsername_NotExistingUser_ReturnsNull(string username)
        {
            var result = await _repository.GetUserByUsername(username);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetUserByUsername_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null);
            
            var result = await _repository.GetUserByUsername("testUsername1");

            result.Should().BeNull();
        }
    }
}