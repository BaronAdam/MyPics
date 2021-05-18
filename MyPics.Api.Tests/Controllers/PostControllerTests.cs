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
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
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
        private Mock<IUserRepository> _userRepository;
        private Mock<IFollowRepository> _followRepository;
        private PostController _controller;
        private Mock<ICloudinaryService> _cloudinaryService;
        private Mock<IFormFile> _formFile;
        private IMapper _mapper;
        
        [SetUp]
        public void Setup()
        {
            _postRepository = new Mock<IPostRepository>();
            _pictureRepository = new Mock<IPictureRepository>();
            _userRepository = new Mock<IUserRepository>();
            _followRepository = new Mock<IFollowRepository>();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<PostForAddDto, Post>();
            });

            _mapper = config.CreateMapper();

            _cloudinaryService = new Mock<ICloudinaryService>();

            _controller = new PostController(_postRepository.Object, _pictureRepository.Object, 
                _userRepository.Object, _followRepository.Object,_cloudinaryService.Object, _mapper)
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

        [Test]
        public async Task GetSinglePost_ExistingPostPublicProfileAcceptedFollow_ReturnsOk()
        {
            SetupRepoGetUserById(false, false);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostForUser(false);

            var result = await _controller.GetSinglePost(1, 1);

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetSinglePost_ExistingPostPrivateProfileAcceptedFollow_ReturnsOk()
        {
            SetupRepoGetUserById(false, true);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostForUser(false);

            var result = await _controller.GetSinglePost(1, 1);

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetSinglePost_ExistingPostPrivateProfileNotAcceptedFollow_ReturnsBadRequest()
        {
            SetupRepoGetUserById(false, true);
            SetupRepoGetFollowStatus(true, false);
            SetupRepoGetPostForUser(false);

            var result = await _controller.GetSinglePost(1, 1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetSinglePost_ExistingPostPrivateProfileNotFollowed_ReturnsBadRequest()
        {
            SetupRepoGetUserById(false, true);
            SetupRepoGetFollowStatus(false, false);
            SetupRepoGetPostForUser(false);

            var result = await _controller.GetSinglePost(1, 1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetSinglePost_NotExistingUser_ReturnsBadRequest()
        {
            SetupRepoGetUserById(true, false);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostForUser(false);

            var result = await _controller.GetSinglePost(1, 1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetSinglePost_NotExistingPost_ReturnsBadRequest()
        {
            SetupRepoGetUserById(false, false);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostForUser(true);

            var result = await _controller.GetSinglePost(1, 1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetPostsForUser_ExistingPostsPublicProfileAcceptedFollow_ReturnsOk()
        {
            SetupRepoGetUserById(false, false);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostsForUser(false);

            var result = await _controller.GetPostsForUser(1, new PostParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetPostsForUser_ExistingPostsPrivateProfileAcceptedFollow_ReturnsOk()
        {
            SetupRepoGetUserById(false, true);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostsForUser(false);

            var result = await _controller.GetPostsForUser(1, new PostParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetPostsForUser_ExistingPostsPrivateProfileNotAcceptedFollow_ReturnsBadRequest()
        {
            SetupRepoGetUserById(false, true);
            SetupRepoGetFollowStatus(true, false);
            SetupRepoGetPostsForUser(false);

            var result = await _controller.GetPostsForUser(1, new PostParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetPostsForUser_ExistingPostsPrivateProfileNotFollowed_ReturnsBadRequest()
        {
            SetupRepoGetUserById(false, true);
            SetupRepoGetFollowStatus(false, false);
            SetupRepoGetPostsForUser(false);

            var result = await _controller.GetPostsForUser(1, new PostParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetPostsForUser_NotExistingUser_ReturnsBadRequest()
        {
            SetupRepoGetUserById(true, false);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostsForUser(false);

            var result = await _controller.GetPostsForUser(1, new PostParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetPostsForUser_NotExistingPost_ReturnsInternalServerError()
        {
            SetupRepoGetUserById(false, false);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetPostsForUser(true);

            var result = await _controller.GetPostsForUser(1, new PostParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetPostsForFeed_ExistingPosts_ReturnsOk()
        {
            SetupRepoGetAllAcceptedFollowIds(false);
            SetupRepoGetPostsForFeed(false);

            var result = await _controller.GetPostsForFeed(new PostParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetPostsForFeed_ExceptionFollows_ReturnsInternalServerError()
        {
            SetupRepoGetAllAcceptedFollowIds(true);
            SetupRepoGetPostsForFeed(false);

            var result = await _controller.GetPostsForFeed(new PostParameters());
            
            result.Should().BeOfType<StatusCodeResult>();
        }
        
        [Test]
        public async Task GetPostsForFeed_ExceptionPosts_ReturnsInternalServerError()
        {
            SetupRepoGetAllAcceptedFollowIds(false);
            SetupRepoGetPostsForFeed(true);

            var result = await _controller.GetPostsForFeed(new PostParameters());
            
            result.Should().BeOfType<StatusCodeResult>();
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

        private void SetupRepoGetUserById(bool shouldReturnNull, bool shouldBePrivate)
        {
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnNull ? new User {IsPrivate = shouldBePrivate} : null);
        }

        private void SetupRepoGetFollowStatus(bool isExisting, bool isAccepted)
        {
            _followRepository.Setup(x => x.GetFollowStatus(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new FollowStatusDto(isExisting, isAccepted));
        }
        
        private void SetupRepoGetAllAcceptedFollowIds(bool shouldReturnNull)
        {
            _followRepository.Setup(x => x.GetAllAcceptedFollowIds(It.IsAny<int>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new List<int>());
        }

        private void SetupRepoGetPostForUser(bool shouldReturnNull)
        {
            _postRepository.Setup(x => x.GetPostForUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnNull ? new PostDto() : null);
        }
        
        private void SetupRepoGetPostsForUser(bool shouldReturnNull)
        {
            var list = new List<PostDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new PostDto {Id = 1});
            }

            _postRepository.Setup(x => x.GetPostsForUser(It.IsAny<int>(), It.IsAny<PostParameters>()))
                .ReturnsAsync(!shouldReturnNull ? new PagedList<PostDto>(list, 1, 1, 1): null);
        }
        
        private void SetupRepoGetPostsForFeed(bool shouldReturnNull)
        {
            var list = new List<PostDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new PostDto {Id = 1});
            }

            _postRepository.Setup(x => x.GetPostsForFeed(It.IsAny<List<int>>(), It.IsAny<PostParameters>()))
                .ReturnsAsync(!shouldReturnNull ? new PagedList<PostDto>(list, 1, 1, 1): null);
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