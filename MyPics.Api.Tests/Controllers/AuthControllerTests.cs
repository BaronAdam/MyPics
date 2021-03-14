using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPics.Api.Controllers;
using MyPics.Domain.DTOs;
using MyPics.Domain.Email;
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
        private Mock<IEmailService> _emailServiceMock;
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
            
            _emailServiceMock = new Mock<IEmailService>();
            _emailServiceMock.Setup(x =>
                    x.BuildConfirmationMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => new EmailMessage());
            _emailServiceMock.Setup(x => x.SendEmail(It.IsAny<EmailMessage>()))
                .ReturnsAsync(() => true);

            Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

            _controller = new AuthController(_repositoryMock.Object, _configurationMock.Object, _mapperMock.Object,
                _emailServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request =
                        {
                            Scheme = ""
                        }
                    }
                },
                Url = urlHelperMock.Object
            };
        }

        [Test]
        public async Task Register_Successful_ReturnsOk()
        {
            SetupRepoRegister(false);
            SetupRepoEmailExists(false);
            SetupRepoUserExists(false);

            var result = await _controller.Register(new UserForRegisterDto
            {
                Username = "testUsername", 
                Email = "email@test.com"
            });

            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task Register_UnSuccessful_ReturnsInternalServerError()
        {
            SetupRepoRegister(true);
            SetupRepoEmailExists(false);
            SetupRepoUserExists(false);

            var result = await _controller.Register(new UserForRegisterDto {
                Username = "testUsername", 
                Email = "email@test.com"
            });

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
            SetupRepoLogin(false, true);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task Login_UnSuccessful_NullUser_ReturnsUnauthorized()
        {
            SetupRepoLogin(true, false);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task Login_UnSuccessful_NotConfirmed_ReturnsUnauthorized()
        {
            SetupRepoLogin(false, false);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
        
        [Test]
        public async Task Login_Exception_ReturnInternalServerError()
        {
            SetupRepoLogin(false, true);
            _configurationMock = new Mock<IConfiguration>();
            _controller = new AuthController(_repositoryMock.Object, _configurationMock.Object, _mapperMock.Object,
                _emailServiceMock.Object);

            var result = await _controller.Login(new UserForLoginDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
        }

        [Test]
        public async Task ConfirmEmail_Successful_ReturnsOk()
        {
            SetupRepoConfirmEmail(true);
            
            var result = await _controller.ConfirmEmail("testToken", "testUsername");

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task ConfirmEmail_UnSuccessful_ReturnsUnauthorized()
        {
            SetupRepoConfirmEmail(false);
            
            var result = await _controller.ConfirmEmail("testToken", "testUsername");

            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
        
        private void SetupRepoLogin(bool shouldReturnNull, bool shouldBeConfirmed)
        {
            _repositoryMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User { 
                    Id = 1, 
                    Email = "email@test.com",
                    Username = "testUsername",
                    RegistrationToken = "testToken",
                    IsConfirmed = shouldBeConfirmed
                });
        }

        private void SetupRepoRegister(bool shouldReturnNull)
        {
            _repositoryMock.Setup(x => x.Register(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User { 
                    Id = 1, 
                    Username = "testUsername",
                    Email = "email@test.com",
                    RegistrationToken = "testToken"
                });
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

        private void SetupRepoConfirmEmail(bool shouldReturnTrue)
        {
            _repositoryMock.Setup(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnTrue);
        }
    }
}