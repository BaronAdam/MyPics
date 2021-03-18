using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyPics.Api.Controllers;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;
using NUnit.Framework;

namespace MyPics.Api.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserRepository> _repository;
        private Mock<IMapper> _mapper;
        private UserController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>();
            
            _mapper.Setup(x => x.ConfigurationProvider)
                .Returns(() => new MapperConfiguration(
                        cfg => { cfg.CreateMap<User, UserForSearchDto>(); }));

            _repository = new Mock<IUserRepository>();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            _controller = new UserController(_repository.Object, _mapper.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user
                    }
                }
            };
        }

        [Test]
        public async Task FindUserByUsername_ExistingUser_ReturnsOk()
        {
            SetupRepoGetUserByUsername(false);

            var result = await _controller.FindUserByUsername("testUsername");

            result.Should().BeOfType<OkObjectResult>();
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task FindUserByUsername_NullOrEmptyString_ReturnsBadRequest(string username)
        {
            SetupRepoGetUserByUsername(false);

            var result = await _controller.FindUserByUsername(username);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task FindUserByUsername_NotExistingUser_ReturnsBadRequest()
        {
            SetupRepoGetUserByUsername(true);

            var result = await _controller.FindUserByUsername("testUsername");

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetFollowsForUser_ExistingFollows_ReturnsOk()
        {
            SetupRepoGetUserFollows(false, false);

            var result = await _controller.GetFollowsForUser(new UserParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetFollowsForUser_NotExistingFollows_ReturnsBadRequest()
        {
            SetupRepoGetUserFollows(false, true);

            var result = await _controller.GetFollowsForUser(new UserParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetFollowsForUser_Exception_ReturnsBadRequest()
        {
            SetupRepoGetUserFollows(true, false);

            var result = await _controller.GetFollowsForUser(new UserParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetFollowersForUser_ExistingFollowers_ReturnsOk()
        {
            SetupRepoGetUserFollowers(false, false);

            var result = await _controller.GetFollowersForUser(new UserParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetFollowersForUser_NotExistingFollowers_ReturnsBadRequest()
        {
            SetupRepoGetUserFollowers(false, true);

            var result = await _controller.GetFollowersForUser(new UserParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetFollowersForUser_Exception_ReturnsBadRequest()
        {
            SetupRepoGetUserFollowers(true, false);

            var result = await _controller.GetFollowersForUser(new UserParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task FindUserInFollows_ExistingUser_ReturnsOk()
        {
            SetupRepoFindUserInFollows(false);

            var result = await _controller.FindUserInFollows("testUsername");

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task FindUserInFollows_NotExistingUserOrException_ReturnsBadRequest()
        {
            SetupRepoFindUserInFollows(true);

            var result = await _controller.FindUserInFollows("testUsername");

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task FindUserInFollowers_ExistingUser_ReturnsOk()
        {
            SetupRepoFindUserInFollowers(false);

            var result = await _controller.FindUserInFollowers("testUsername");

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task FindUserInFollowers_NotExistingUserOrException_ReturnsBadRequest()
        {
            SetupRepoFindUserInFollowers(true);

            var result = await _controller.FindUserInFollowers("testUsername");

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        private void SetupRepoGetUserByUsername(bool shouldReturnNull)
        {
            _repository.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User());
        }
        
        private void SetupRepoGetUserFollows(bool shouldReturnNull, bool shouldReturnEmpty)
        {
            if (shouldReturnEmpty)
            {
                _repository.Setup(x => x.GetUserFollows(It.IsAny<int>(), It.IsAny<UserParameters>()))
                    .ReturnsAsync(() => new PagedList<UserForFollowDto>(new List<UserForFollowDto>(), 0, 0, 0));
                return;
            }

            var list = new List<UserForFollowDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new UserForFollowDto{ Username = "test"});
            }

            _repository.Setup(x => x.GetUserFollows(It.IsAny<int>(), It.IsAny<UserParameters>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new PagedList<UserForFollowDto>(list, 1, 10, 10));
        }
        
        private void SetupRepoGetUserFollowers(bool shouldReturnNull, bool shouldReturnEmpty)
        {
            if (shouldReturnEmpty)
            {
                _repository.Setup(x => x.GetUserFollowers(It.IsAny<int>(), It.IsAny<UserParameters>()))
                    .ReturnsAsync(() => new PagedList<UserForFollowDto>(new List<UserForFollowDto>(), 0, 0, 0));
                return;
            }

            var list = new List<UserForFollowDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new UserForFollowDto{ Username = "test"});
            }

            _repository.Setup(x => x.GetUserFollowers(It.IsAny<int>(), It.IsAny<UserParameters>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new PagedList<UserForFollowDto>(list, 1, 10, 10));
        }
        
        private void SetupRepoFindUserInFollows(bool shouldReturnNull)
        {
            _repository.Setup(x => x.FindUserInFollows(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new UserForFollowDto());
        }
        
        private void SetupRepoFindUserInFollowers(bool shouldReturnNull)
        {
            _repository.Setup(x => x.FindUserInFollowers(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new UserForFollowDto());
        }
    }
}