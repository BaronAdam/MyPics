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
        private Mock<IUserRepository> _userRepository;
        private Mock<IMapper> _mapper;
        private UserController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mapper = new Mock<IMapper>();
            
            _mapper.Setup(x => x.ConfigurationProvider)
                .Returns(() => new MapperConfiguration(
                        cfg => { cfg.CreateMap<User, UserForSearchDto>(); }));

            _userRepository = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            _controller = new UserController(_userRepository.Object, _mapper.Object)
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

        private void SetupRepoGetUserByUsername(bool shouldReturnNull)
        {
            _userRepository.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User());
        }
    }
}