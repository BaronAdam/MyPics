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
    public class CommentControllerTests
    {
        private Mock<IFollowRepository> _followRepository;
        private Mock<IPostRepository> _postRepository;
        private Mock<ICommentRepository> _commentRepository;
        private Mock<IMapper> _mapper;
        private CommentController _controller;
        
        [SetUp]
        public void Setup()
        {
            _followRepository = new Mock<IFollowRepository>();
            _postRepository = new Mock<IPostRepository>();
            _commentRepository = new Mock<ICommentRepository>();
            _mapper = new Mock<IMapper>();

            _mapper.Setup(x => x.Map<Comment>(It.IsAny<CommentForAddDto>()))
                .Returns(new Comment());
            _mapper.Setup(x => x.Map<CommentForSingleDto>(It.IsNotNull<Comment>()))
                .Returns(new CommentForSingleDto());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            _controller = new CommentController(_commentRepository.Object, _postRepository.Object, 
                _followRepository.Object, _mapper.Object)
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
        public async Task CommentPost_CorrectComment_ReturnsOk()
        {
            SetupRepoAdd(false);
            
            var result = await _controller.CommentPost(new CommentForAddDto());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task CommentPost_NullComment_ReturnsBadRequest()
        {
            SetupRepoAdd(false);
            
            var result = await _controller.CommentPost(null);

            result.Should().BeOfType<BadRequestResult>();
        }
        
        [Test]
        public async Task CommentPost_Exception_ReturnsInternalServerError()
        {
            SetupRepoAdd(true);
            
            var result = await _controller.CommentPost(new CommentForAddDto());

            result.Should().BeOfType<StatusCodeResult>();
        }

        [Test]
        public async Task UpdateComment_ExistingComment_ReturnsOk()
        {
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoUpdate(false);

            var result = await _controller.UpdateComment(new CommentForEditDto());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task UpdateComment_NotExistingComment_ReturnsBadRequest()
        {
            SetupRepoGetByIdComment(true, false, 123);
            SetupRepoUpdate(false);

            var result = await _controller.UpdateComment(new CommentForEditDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task UpdateComment_ExistingCommentDeleted_ReturnsBadRequest()
        {
            SetupRepoGetByIdComment(false, true, 123);
            SetupRepoUpdate(false);

            var result = await _controller.UpdateComment(new CommentForEditDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task UpdateComment_ExistingCommentWrongUser_ReturnsUnauthorized()
        {
            SetupRepoGetByIdComment(false, false, 1000);
            SetupRepoUpdate(false);

            var result = await _controller.UpdateComment(new CommentForEditDto());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task UpdateComment_ExistingCommentWrongUser_ReturnsInternalServerError()
        {
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoUpdate(true);

            var result = await _controller.UpdateComment(new CommentForEditDto());

            result.Should().BeOfType<StatusCodeResult>();
        }

        [Test]
        public async Task DeleteComment_ExistingComment_ReturnsOk()
        {
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoRemove(false);

            var result = await _controller.DeleteComment(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task DeleteComment_NotExistingComment_ReturnsBadRequest()
        {
            SetupRepoGetByIdComment(true, false, 123);
            SetupRepoRemove(false);

            var result = await _controller.DeleteComment(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task DeleteComment_ExistingCommentDeleted_ReturnsBadRequest()
        {
            SetupRepoGetByIdComment(false, true, 123);
            SetupRepoRemove(false);

            var result = await _controller.DeleteComment(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task DeleteComment_ExistingCommentWrongUser_ReturnsUnauthorized()
        {
            SetupRepoGetByIdComment(false, false, 1000);
            SetupRepoRemove(false);

            var result = await _controller.DeleteComment(1);

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task DeleteComment_Exception_ReturnsInternalServerError()
        {
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoRemove(true);

            var result = await _controller.DeleteComment(1);

            result.Should().BeOfType<StatusCodeResult>();
        }

        [Test]
        public async Task GetCommentsForPost_ExistingPublicPost_ReturnsOk()
        {
            SetupRepoGetByIdPost(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetCommentsForPost(false);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetCommentsForPost_ExistingPrivatePostUserIsAuthor_ReturnsOk()
        {
            SetupRepoGetByIdPost(false, true, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetCommentsForPost(false);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetCommentsForPost_ExistingPrivatePost_ReturnsOk()
        {
            SetupRepoGetByIdPost(false, true, 1000);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetCommentsForPost(false);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetCommentsForPost_ExistingPrivatePostNotInFollows_ReturnsUnauthorized()
        {
            SetupRepoGetByIdPost(false, true, 1000);
            SetupRepoGetFollowStatus(false, false);
            SetupRepoGetCommentsForPost(false);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task GetCommentsForPost_ExistingPrivatePostNotAcceptedFollow_ReturnsUnauthorized()
        {
            SetupRepoGetByIdPost(false, true, 1000);
            SetupRepoGetFollowStatus(true, false);
            SetupRepoGetCommentsForPost(false);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task GetCommentsForPost_NotExistingPost_ReturnsBadRequest()
        {
            SetupRepoGetByIdPost(true, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetCommentsForPost(false);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetCommentsForPost_Exception_ReturnsInternalServerError()
        {
            SetupRepoGetByIdPost(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetCommentsForPost(true);

            var result = await _controller.GetCommentsForPost(1, new CommentParameters());

            result.Should().BeOfType<StatusCodeResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_ExistingPublicPost_ReturnsOk()
        {
            SetupRepoGetByIdPost(false, false, 123);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetRepliesForComment(false);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_ExistingPrivatePostUserIsAuthor_ReturnsOk()
        {
            SetupRepoGetByIdPost(false, true, 123);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetRepliesForComment(false);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_ExistingPrivatePost_ReturnsOk()
        {
            SetupRepoGetByIdPost(false, true, 1000);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetRepliesForComment(false);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_ExistingPrivatePostNotInFollows_ReturnsUnauthorized()
        {
            SetupRepoGetByIdPost(false, true, 1000);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(false, false);
            SetupRepoGetRepliesForComment(false);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_ExistingPrivatePostNotAccepted_ReturnsUnauthorized()
        {
            SetupRepoGetByIdPost(false, true, 1000);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(true, false);
            SetupRepoGetRepliesForComment(false);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_NotExistingPost_ReturnsBadRequest()
        {
            SetupRepoGetByIdPost(true, false, 123);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetRepliesForComment(false);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetRepliesForComment_Exception_ReturnsInternalServerError()
        {
            SetupRepoGetByIdPost(false, false, 123);
            SetupRepoGetByIdComment(false, false, 123);
            SetupRepoGetFollowStatus(true, true);
            SetupRepoGetRepliesForComment(true);

            var result = await _controller.GetRepliesForComment(1, new CommentParameters());

            result.Should().BeOfType<StatusCodeResult>();
        }

            private void SetupRepoAdd(bool shouldReturnNull)
        {
            _commentRepository.Setup(x => x.Add(It.IsAny<Comment>()))
                .ReturnsAsync(shouldReturnNull ? null : new Comment());
        }

        private void SetupRepoUpdate(bool shouldReturnNull)
        {
            _commentRepository.Setup(x => x.Update(It.IsAny<CommentForEditDto>()))
                .ReturnsAsync(shouldReturnNull ? null : new Comment());
        }

        private void SetupRepoRemove(bool shouldReturnFalse)
        {
            _commentRepository.Setup(x => x.Remove(It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupRepoGetByIdPost(bool shouldReturnNull, bool shouldBePrivate, int userId)
        {
            _postRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull
                    ? null
                    : new Post
                    {
                        UserId = userId,
                        User = new User
                        {
                            Id = userId,
                            IsPrivate = shouldBePrivate
                        }
                    });
        }

        private void SetupRepoGetByIdComment(bool shouldReturnNull, bool shouldBeDeleted, int userId)
        {
            _commentRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull
                    ? null
                    : new Comment
                    {
                        UserId = userId,
                        IsDeleted = shouldBeDeleted
                    });
        }
        
        private void SetupRepoGetFollowStatus(bool isExisting, bool isAccepted)
        {
            _followRepository.Setup(x => x.GetFollowStatus(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new FollowStatusDto(isExisting, isAccepted));
        }

        private void SetupRepoGetCommentsForPost(bool shouldReturnNull)
        {
            var list = new List<CommentForListDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new CommentForListDto
                {
                    Id = i,
                    IsDeleted = true,
                    User = new UserForCommentDto()
                });
            }

            _commentRepository.Setup(x => x.GetCommentsForPost(It.IsAny<int>(), It.IsAny<CommentParameters>()))
                .ReturnsAsync(shouldReturnNull ? null : new PagedList<CommentForListDto>(list, 1, 10, 10));
        }
        
        private void SetupRepoGetRepliesForComment(bool shouldReturnNull)
        {
            var list = new List<CommentForListDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new CommentForListDto
                {
                    Id = i,
                    IsDeleted = true,
                    User = new UserForCommentDto()
                });
            }

            _commentRepository.Setup(x => x.GetRepliesForComment(It.IsAny<int>(), It.IsAny<CommentParameters>()))
                .ReturnsAsync(shouldReturnNull ? null : new PagedList<CommentForListDto>(list, 1, 10, 10));
        }
    }
}