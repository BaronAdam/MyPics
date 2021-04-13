using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
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
using MyPics.Infrastructure.Persistence;
using NUnit.Framework;

namespace MyPics.Api.Tests.Controllers
{
    [TestFixture]
    public class PostControllerTests
    {
        private Mock<IPostRepository> _postRepository;
        private Mock<IPictureRepository> _pictureRepository;
        private PostController _controller;
        private Mock<ICloudinaryService> _cloudinaryService;
        private Mock<IFormFile> _formFile;
        
        [SetUp]
        public void Setup()
        {
            _postRepository = new Mock<IPostRepository>();
            _pictureRepository = new Mock<IPictureRepository>();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<PostForAddDto, Post>();
            });

            var mapper = config.CreateMapper();

            _cloudinaryService = new Mock<ICloudinaryService>();

            _controller = new PostController(_postRepository.Object, _pictureRepository.Object, 
                _cloudinaryService.Object, mapper)
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
        public async Task AddPost_CorrectPost_ReturnsOk()
        {
            SetupRepoAddPost(false);
            SetupRepoDeletePost(false);
            SetupRepoAddPicturesForPost(false);
            SetupServiceUploadFile(false);
        
            var dto = new PostForAddDto
            {
                Description = "test",
                NumberOfPictures = 1,
                Files = new List<IFormFile>
                {
                    _formFile.Object
                }
            };
        
            var result = await _controller.AddPost(dto);
        
            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task AddPost_InCorrectPost_ReturnsBadRequest()
        {
            SetupRepoAddPost(false);
            SetupRepoDeletePost(false);
            SetupRepoAddPicturesForPost(false);
            SetupServiceUploadFile(false);
        
            var dto = new PostForAddDto
            {
                Description = "test",
                NumberOfPictures = 1,
                Files = new List<IFormFile>()
            };
        
            var result = await _controller.AddPost(dto);
        
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task AddPost_FailWhileAddingPost_ReturnsBadRequest()
        {
            SetupRepoAddPost(true);
            SetupRepoDeletePost(false);
            SetupRepoAddPicturesForPost(false);
            SetupServiceUploadFile(false);
        
            var dto = new PostForAddDto
            {
                Description = "test",
                NumberOfPictures = 1,
                Files = new List<IFormFile>()
                {
                    _formFile.Object
                }
            };
        
            var result = await _controller.AddPost(dto);
        
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task AddPost_FailWhileAddingPictures_ReturnsBadRequest()
        {
            SetupRepoAddPost(false);
            SetupRepoDeletePost(false);
            SetupRepoAddPicturesForPost(true);
            SetupServiceUploadFile(false);
        
            var dto = new PostForAddDto
            {
                Description = "test",
                NumberOfPictures = 1,
                Files = new List<IFormFile>()
                {
                    _formFile.Object
                }
            };
        
            var result = await _controller.AddPost(dto);
        
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task AddPost_FailWhileUploading_ReturnsBadRequest()
        {
            SetupRepoAddPost(false);
            SetupRepoDeletePost(false);
            SetupRepoAddPicturesForPost(false);
            SetupServiceUploadFile(true);
        
            var dto = new PostForAddDto
            {
                Description = "test",
                NumberOfPictures = 1,
                Files = new List<IFormFile>()
                {
                    _formFile.Object
                }
            };
        
            var result = await _controller.AddPost(dto);
        
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task DeletePost_ExistingPost_ReturnsOk()
        {
            SetupRepoDeletePost(false);

            var result = await _controller.DeletePost(1);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task DeletePost_NotExistingPost_ReturnsBadRequest()
        {
            SetupRepoDeletePost(true);

            var result = await _controller.DeletePost(1);

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task UpdatePost_ExistingPost_ReturnsOk()
        {
            SetupRepoUpdatePost(false);

            var result = await _controller.UpdatePost(new PostForUpdateDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task UpdatePost_NotExistingPost_ReturnsBadRequest()
        {
            SetupRepoUpdatePost(true);

            var result = await _controller.UpdatePost(new PostForUpdateDto());

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        private void SetupRepoAddPost(bool shouldReturnNull)
        {
            _postRepository.Setup(x => x.AddPost(It.IsAny<Post>()))
                .ReturnsAsync(shouldReturnNull ? null : new Post {Id = 1});
        }

        private void SetupRepoDeletePost(bool shouldReturnFalse)
        {
            _postRepository.Setup(x => x.DeletePost(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupRepoUpdatePost(bool shouldReturnFalse)
        {
            _postRepository.Setup(x => x.EditPost(It.IsAny<PostForUpdateDto>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupRepoAddPicturesForPost(bool shouldReturnFalse)
        {
            _pictureRepository.Setup(x => x.AddPicturesForPost(It.IsAny<List<Picture>>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupServiceUploadFile(bool shouldReturnNull)
        {
            var uploadResult = new ImageUploadResult
            {
                PublicId = "testId",
                Url = new Uri("https://localhost:5001")
            };

            _cloudinaryService.Setup(x => x.UploadFile(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(shouldReturnNull ? null : new CustomUploadResult(uploadResult));
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