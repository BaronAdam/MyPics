using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPics.Api.Controllers;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;
using NUnit.Framework;

namespace MyPics.Api.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthRepository> _repositoryMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IMapper> _mapperMock;
        private AuthController _controller; 
        
        [SetUp]
        public void Setup()
        {
            var mockKeySection = new Mock<IConfigurationSection>();
            mockKeySection.Setup(c => c.Value)
                .Returns("TAf30yv4g15177S6EW6idxfE5YxyJiCX8Wf2c4nf9Aw=");
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(() => mockKeySection.Object);

            _repositoryMock = new Mock<IAuthRepository>();

            _mapperMock = new Mock<IMapper>();

            _mapperMock.Setup(x => x.Map<User>(It.IsAny<UserForRegisterDto>()))
                .Returns(new User());

            _controller = new AuthController(_repositoryMock.Object, _configurationMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Register_Successful_ReturnsOk()
        {
            SetupRepoRegister(false);
            SetupRepoEmailExists(false);
            SetupRepoUserExists(false);

            var result = await _controller.Register(new UserForRegisterDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task Register_UnSuccessful_ReturnsInternalServerError()
        {
            SetupRepoRegister(true);
            SetupRepoEmailExists(false);
            SetupRepoUserExists(false);

            var result = await _controller.Register(new UserForRegisterDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }
        
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Register_UnSuccessful_ReturnsBadRequest(bool userExists, bool emailExists)
        {
            SetupRepoRegister(false);
            SetupRepoEmailExists(userExists);
            SetupRepoUserExists(emailExists);

            var result = await _controller.Register(new UserForRegisterDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Login_Successful_ReturnsOk()
        {
            SetupRepoLogin(false);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task Login_UnSuccessful_ReturnsUnauthorized()
        {
            SetupRepoLogin(true);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task Login_Exception_ReturnInternalServerError()
        {
            SetupRepoLogin(false);
            _configurationMock = new Mock<IConfiguration>();
            _controller = new AuthController(_repositoryMock.Object, _configurationMock.Object, _mapperMock.Object);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }

        private void SetupRepoLogin(bool shouldReturnNull)
        {
            _repositoryMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User{ Id = 1, Username = "testUsername" });
        }

        private void SetupRepoRegister(bool shouldReturnNull)
        {
            _repositoryMock.Setup(x => x.Register(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User());
        }

        private void SetupRepoUserExists(bool shouldReturnTrue)
        {
            _repositoryMock.Setup(x => x.UserExists(It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnTrue);
        }
        
        private void SetupRepoEmailExists(bool shouldReturnTrue)
        {
            _repositoryMock.Setup(x => x.EmailExists(It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnTrue);
        }
    }
}