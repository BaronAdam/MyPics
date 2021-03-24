using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyPics.Api.Controllers;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;
using NUnit.Framework;

namespace MyPics.Api.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<ICloudinaryService> _cloudinaryService;
        private Mock<IMapper> _mapper;
        private UserController _controller;
        private Mock<IFormFile> _formFile;
        
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

            _cloudinaryService = new Mock<ICloudinaryService>();

            _controller = new UserController(_userRepository.Object, _mapper.Object, _cloudinaryService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user
                    }
                }
            };
            
            _formFile = new Mock<IFormFile>();
            SetupFileMock();
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
        public async Task UploadProfilePicture_Successful_ReturnsOk()
        {
            SetupServiceUploadImageAsync(false);
            SetupRepoChangeProfilePicture(false);

            var result = await _controller.UploadProfilePicture(new ProfilePictureDto {File = _formFile.Object});

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task UploadProfilePicture_UnSuccessfulNullFile_ReturnsBadRequest()
        {
            SetupServiceUploadImageAsync(false);
            SetupRepoChangeProfilePicture(false);
            _formFile = new Mock<IFormFile>();

            var result = await _controller.UploadProfilePicture(new ProfilePictureDto {File = _formFile.Object});

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task UploadProfilePicture_UnSuccessfulNullUploadResult_ReturnsBadRequest()
        {
            SetupServiceUploadImageAsync(true);
            SetupRepoChangeProfilePicture(false);

            var result = await _controller.UploadProfilePicture(new ProfilePictureDto {File = _formFile.Object});

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task UploadProfilePicture_UnSuccessfulNullRepoResult_ReturnsBadRequest()
        {
            SetupServiceUploadImageAsync(false);
            SetupRepoChangeProfilePicture(true);

            var result = await _controller.UploadProfilePicture(new ProfilePictureDto {File = _formFile.Object});

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private void SetupRepoGetUserByUsername(bool shouldReturnNull)
        {
            _userRepository.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new User());
        }

        private void SetupRepoChangeProfilePicture(bool shouldReturnFalse)
        {
            _userRepository.Setup(x => x.ChangeProfilePicture(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(!shouldReturnFalse);
        }

        private void SetupServiceUploadImageAsync(bool shouldReturnNull)
        {
            var uploadResult = new ImageUploadResult
            {
                PublicId = "testId",
                Url = new Uri("https://localhost:5001")
            };

            _cloudinaryService.Setup(x => x.UploadImageAsync(It.IsAny<ImageUploadParams>()))
                .ReturnsAsync(shouldReturnNull ? null : uploadResult);
        }

        private void SetupFileMock()
        {
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            _formFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            _formFile.Setup(_ => _.FileName).Returns(fileName);
            _formFile.Setup(_ => _.Length).Returns(ms.Length);
        }
    }
}