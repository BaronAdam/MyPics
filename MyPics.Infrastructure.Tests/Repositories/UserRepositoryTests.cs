using System;
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
    public class UserRepositoryTests
    {
        private MyPicsDbContext _context;
        private UserRepository _repository;
        private Mock<IMapper> _mapper;

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

            _mapper = new Mock<IMapper>();
            
            _mapper.Setup(x => x.ConfigurationProvider)
                .Returns(
                    () => new MapperConfiguration(
                        cfg => { cfg.CreateMap<User, UserForFollowDto>(); }));


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

            _context.Follows.Add(new Follow
            {
                UserId = 1,
                FollowingId = 2
            });

            _context.Follows.Add(new Follow
            {
                UserId = 3,
                FollowingId = 1
            });
            
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _repository = new UserRepository(_context, _mapper.Object);
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
        
        [TestCase(1000)]
        [TestCase(2000)]
        [TestCase(3000)]
        public async Task GetUserById_NotExistingUser_ReturnsNull(int id)
        {
            var result = await _repository.GetUserById(id);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetUserById_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null, _mapper.Object);
            
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
            _repository = new UserRepository(null, _mapper.Object);
            
            var result = await _repository.GetUserByUsername("testUsername1");

            result.Should().BeNull();
        }

        [Test]
        public async Task GetUserFollows_ExistingFollows_ReturnsExpected()
        {
            var result = await _repository.GetUserFollows(1, new UserParameters());

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedList<UserForFollowDto>>();
        }
        
        [Test]
        public async Task GetUserFollows_NotExistingFollows_ReturnsEmpty()
        {
            var result = await _repository.GetUserFollows(2, new UserParameters());

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserFollows_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null, null);
            
            var result = await _repository.GetUserFollows(1, new UserParameters());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetUserFollowers_ExistingFollowers_ReturnsExpected()
        {
            var result = await _repository.GetUserFollowers(1, new UserParameters());

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedList<UserForFollowDto>>();
        }
        
        [Test]
        public async Task GetUserFollowers_NotExistingFollowers_ReturnsEmpty()
        {
            var result = await _repository.GetUserFollowers(3, new UserParameters());

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserFollowers_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null, null);
            
            var result = await _repository.GetUserFollowers(1, new UserParameters());

            result.Should().BeNull();
        }

        // [Test]
        // public async Task FindUserInFollows_ExistingUser_ReturnsUser()
        // {
        //     var result = await _repository.FindUserInFollows(1, "testUsername2");
        //
        //     result.Should().NotBeNull();
        //     result.Should().BeOfType<UserForFollowDto>();
        //     result.Username.Should().Be("testUsername2");
        // }
        
        [Test]
        public async Task FindUserInFollows_NotExistingUser_ReturnsNull()
        {
            var result = await _repository.FindUserInFollows(1, "notExistingTestUsername");

            result.Should().BeNull();
        }
        
        [Test]
        public async Task FindUserInFollows_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null, null);
            
            var result = await _repository.FindUserInFollows(1, "testUsername2");

            result.Should().BeNull();
        }
        
        // [Test]
        // public async Task FindUserInFollowers_ExistingUser_ReturnsUser()
        // {
        //     var result = await _repository.FindUserInFollowers(1, "testUsername3");
        //
        //     result.Should().NotBeNull();
        //     result.Should().BeOfType<UserForFollowDto>();
        //     result.Username.Should().Be("testUsername3");
        // }
        
        [Test]
        public async Task FindUserInFollowers_NotExistingUser_ReturnsNull()
        {
            var result = await _repository.FindUserInFollowers(1, "notExistingTestUsername");

            result.Should().BeNull();
        }
        
        [Test]
        public async Task FindUserInFollowers_Exception_ReturnsNull()
        {
            _repository = new UserRepository(null, null);
            
            var result = await _repository.FindUserInFollowers(1, "testUsername3");

            result.Should().BeNull();
        }
    }
}