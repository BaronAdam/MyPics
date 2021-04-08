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
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            var mapper = new Mock<IMapper>();
            
            mapper.Setup(x => x.ConfigurationProvider)
                .Returns(() => new MapperConfiguration(
                    cfg => { cfg.CreateMap<PostForAddDto, Post>()
                        .ForSourceMember(x => x.Files, opt => opt.DoNotValidate());; }));

            _cloudinaryService = new Mock<ICloudinaryService>();

            _controller = new PostController(_postRepository.Object, _pictureRepository.Object, 
                _cloudinaryService.Object, mapper.Object)
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

        // [Test]
        // public async Task AddPost_CorrectPost_ReturnsOk()
        // {
        //     SetupRepoAddPost(false);
        //     SetupRepoDeletePost(false);
        //     SetupRepoAddPicturesForPost(false);
        //     SetupServiceUploadImageAsync(false);
        //
        //     var dto = new PostForAddDto
        //     {
        //         Description = "test",
        //         NumberOfPictures = 1,
        //         Files = new List<IFormFile>
        //         {
        //             _formFile.Object
        //         }
        //     };
        //
        //     var result = await _controller.AddPost(dto);
        //
        //     result.Should().NotBeNull();
        //     result.Should().NotBeOfType<OkResult>();
        // }

        private void SetupRepoAddPost(bool shouldReturnNull)
        {
            _postRepository.Setup(x => x.AddPost(It.IsAny<Post>()))
                .ReturnsAsync(shouldReturnNull ? null : new Post {Id = 1});
        }

        private void SetupRepoDeletePost(bool shouldReturnFalse)
        {
            _postRepository.Setup(x => x.DeletePost(It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupRepoAddPicturesForPost(bool shouldReturnFalse)
        {
            _pictureRepository.Setup(x => x.AddPicturesForPost(It.IsAny<List<Picture>>()))
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