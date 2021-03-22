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
    public class FollowRepositoryTests
    {
        private MyPicsDbContext _context;
        private FollowRepository _repository;

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
                        cfg => { cfg.CreateMap<User, UserForFollowDto>(); }));
            
            _context = new MyPicsDbContext(options, configuration.Object);

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
            _context.Users.Add(new User
            {
                Id = 4,
                Username = "testUsername4",
                Email = "test4@email.com",
            });
            _context.Users.Add(new User
            {
                Id = 5,
                Username = "testUsername5",
                Email = "test5@email.com",
                IsPrivate = true
            });
            _context.Users.Add(new User
            {
                Id = 6,
                Username = "testUsername5",
                Email = "test5@email.com",
            });

            _context.Follows.Add(new Follow
            {
                UserId = 1,
                FollowingId = 2,
                IsAccepted = true
            });
            _context.Follows.Add(new Follow
            {
                UserId = 1,
                FollowingId = 3,
                IsAccepted = false
            });
            _context.Follows.Add(new Follow
            {
                UserId = 3,
                FollowingId = 1,
                IsAccepted = false
            });

            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _repository = new FollowRepository(_context, mapper.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
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
            _repository = new FollowRepository(null, null);
            
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
            var result = await _repository.GetUserFollowers(5, new UserParameters());

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserFollowers_Exception_ReturnsNull()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.GetUserFollowers(1, new UserParameters());

            result.Should().BeNull();
        }

        [Test]
        public async Task FindUserInFollows_ExistingUser_ReturnsUser()
        {
            var result = await _repository.FindUserInFollows(1, "testUsername2");
        
            result.Should().NotBeNull();
            result.Should().BeOfType<UserForFollowDto>();
            result.Username.Should().Be("testUsername2");
        }
        
        [Test]
        public async Task FindUserInFollows_NotExistingUser_ReturnsNull()
        {
            var result = await _repository.FindUserInFollows(1, "notExistingTestUsername");

            result.Should().BeNull();
        }
        
        [Test]
        public async Task FindUserInFollows_Exception_ReturnsNull()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.FindUserInFollows(1, "testUsername2");

            result.Should().BeNull();
        }
        
        [Test]
        public async Task FindUserInFollowers_ExistingUser_ReturnsUser()
        {
            var result = await _repository.FindUserInFollowers(1, "testUsername3");
        
            result.Should().NotBeNull();
            result.Should().BeOfType<UserForFollowDto>();
            result.Username.Should().Be("testUsername3");
        }
        
        [Test]
        public async Task FindUserInFollowers_NotExistingUser_ReturnsNull()
        {
            var result = await _repository.FindUserInFollowers(1, "notExistingTestUsername");

            result.Should().BeNull();
        }
        
        [Test]
        public async Task FindUserInFollowers_Exception_ReturnsNull()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.FindUserInFollowers(1, "testUsername3");

            result.Should().BeNull();
        }

        [Test]
        public async Task FollowUser_Successful_ReturnsTrue()
        {
            var result = await _repository.FollowUser(5, 6);
        
            result.Should().BeTrue();
        }
        
        [Test]
        public async Task FollowUser_UnSuccessfulExisting_ReturnsFalse()
        {
            var result = await _repository.FollowUser(1, 2);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task FollowUser_Exception_ReturnsFalse()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.FollowUser(1, 2);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetFollowStatus_ExistingAccepted_ReturnsExpected()
        {
            var result = await _repository.GetFollowStatus(1, 2);

            result.Should().BeOfType<FollowStatusDto>();
            result.IsFollowAccepted.Should().BeTrue();
            result.IsAlreadyInFollows.Should().BeTrue();
        }
        
        [Test]
        public async Task GetFollowStatus_ExistingNotAccepted_ReturnsExpected()
        {
            var result = await _repository.GetFollowStatus(1, 3);

            result.Should().BeOfType<FollowStatusDto>();
            result.IsFollowAccepted.Should().BeFalse();
            result.IsAlreadyInFollows.Should().BeTrue();
        }
        
        [Test]
        public async Task GetFollowStatus_NotExisting_ReturnsExpected()
        {
            var result = await _repository.GetFollowStatus(1, 100);

            result.Should().BeOfType<FollowStatusDto>();
            result.IsFollowAccepted.Should().BeFalse();
            result.IsAlreadyInFollows.Should().BeFalse();
        }
        
        [Test]
        public async Task GetFollowStatus_Exception_ReturnsExpected()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.GetFollowStatus(1, 2);

            result.Should().BeOfType<FollowStatusDto>();
            result.IsFollowAccepted.Should().BeFalse();
            result.IsAlreadyInFollows.Should().BeFalse();
        }

        [Test]
        public async Task UnFollowUser_ExistingFollow_ReturnsTrue()
        {
            var result = await _repository.UnFollowUser(1, 3);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task UnFollowUser_NotExistingFollow_ReturnsFalse()
        {
            var result = await _repository.UnFollowUser(1, 100);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task UnFollowUser_Exception_ReturnsFalse()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.UnFollowUser(1, 3);

            result.Should().BeFalse();
        }

        [Test]
        public async Task AcceptFollow_ExistingFollow_ReturnsTrue()
        {
            var result = await _repository.AcceptFollow(1, 3);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task AcceptFollow_NotExistingFollow_ReturnsFalse()
        {
            var result = await _repository.AcceptFollow(1, 100);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task AcceptFollow_ExistingFollowAlreadyAccepted_ReturnsFalse()
        {
            var result = await _repository.AcceptFollow(1, 2);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task AcceptFollow_Exception_ReturnsFalse()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.AcceptFollow(1, 3);

            result.Should().BeFalse();
        }

        [Test]
        public async Task RejectFollow_ExistingFollow_ReturnsTrue()
        {
            var result = await _repository.RejectFollow(1, 3);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task RejectFollow_ExistingFollowAlreadyAccepted_ReturnsFalse()
        {
            var result = await _repository.RejectFollow(2, 1);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task RejectFollow_Exception_ReturnsFalse()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.RejectFollow(1, 3);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task RemoveFollower_ExistingFollower_ReturnsTrue()
        {
            var result = await _repository.RemoveFollower(1, 3);

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task RejectFollower_NotExistingFollower_ReturnsFalse()
        {
            var result = await _repository.RemoveFollower(2, 10);

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task RemoveFollower_Exception_ReturnsFalse()
        {
            _repository = new FollowRepository(null, null);
            
            var result = await _repository.RemoveFollower(1, 3);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetNotAcceptedFollows_ExistingFollows_ReturnsExpected()
        {
            var parameters = new UserParameters();

            var result = await _repository.GetNotAcceptedFollows(parameters, 1);

            result.Should().BeOfType<PagedList<UserForFollowDto>>();
            result[0].Id.Should().Be(3);
        }
        
        [Test]
        public async Task GetNotAcceptedFollows_NotExistingFollows_ReturnsExpected()
        {
            var parameters = new UserParameters();

            var result = await _repository.GetNotAcceptedFollows(parameters, 5);

            result.Should().BeOfType<PagedList<UserForFollowDto>>();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetNotAcceptedFollows_NotPrivateProfile_ReturnsExpected()
        {
            var parameters = new UserParameters();

            var result = await _repository.GetNotAcceptedFollows(parameters, 6);

            result.Should().BeOfType<PagedList<UserForFollowDto>>();
            result.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetNotAcceptedFollows_Exception_ReturnsNull()
        {
            _repository = new FollowRepository(null, null);
            
            var parameters = new UserParameters();

            var result = await _repository.GetNotAcceptedFollows(parameters, 10);

            result.Should().BeNull();
        }
    }
}