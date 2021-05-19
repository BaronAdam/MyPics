using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class LikeControllerTests
    {
        private Mock<ILikeRepository> _likeRepository;
        private Mock<IPostRepository> _postRepository;
        private Mock<IFollowRepository> _followRepository;
        private Mock<ICommentRepository> _commentRepository;
        private LikeController _controller;
        
        [SetUp]
        public void Setup()
        {
            _likeRepository = new Mock<ILikeRepository>();
            _postRepository = new Mock<IPostRepository>();
            _followRepository = new Mock<IFollowRepository>();
            _commentRepository = new Mock<ICommentRepository>();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            _controller = new LikeController(_likeRepository.Object, _postRepository.Object, _followRepository.Object,
                _commentRepository.Object)
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
        public async Task LikePost_ExistingPost_ReturnsOk()
        {
            SetupLikeRepoAddPostLike(false);
            SetupPostRepoGetById(false, false);

            var result = await _controller.LikePost(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task LikePost_ExistingPostPrivateUser_ReturnsOk()
        {
            SetupLikeRepoAddPostLike(false);
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(false, true, true);

            var result = await _controller.LikePost(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public async Task LikePost_ExistingPostPrivateUserBadFollow_ReturnsUnauthorized(bool isInFollows, bool isAccepted)
        {
            SetupLikeRepoAddPostLike(false);
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(false, isInFollows, isAccepted);

            var result = await _controller.LikePost(1);

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task LikePost_ExistingPostPrivateUserNullFollow_ReturnsBadRequest()
        {
            SetupLikeRepoAddPostLike(false);
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(true, true, true);

            var result = await _controller.LikePost(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task LikePost_NotExistingPost_ReturnsBadRequest()
        {
            SetupLikeRepoAddPostLike(false);
            SetupPostRepoGetById(true, true);

            var result = await _controller.LikePost(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task LikePost_ExistingPostError_ReturnsBadRequest()
        {
            SetupLikeRepoAddPostLike(true);
            SetupPostRepoGetById(false, false);

            var result = await _controller.LikePost(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task LikeComment_ExistingComment_ReturnsOk()
        {
            SetupLikeRepoAddCommentLike(false);
            SetupPostRepoGetById(false, false);
            SetupCommentRepoGetById(false);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task LikeComment_ExistingCommentPrivateUser_ReturnsOk()
        {
            SetupLikeRepoAddCommentLike(false);
            SetupPostRepoGetById(false, true);
            SetupCommentRepoGetById(false);
            SetupFollowRepoGetFollowStatus(false, true, true);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public async Task LikeComment_ExistingCommentPrivateUserBadFollow_ReturnsUnauthorized(bool isInFollows, bool isAccepted)
        {
            SetupLikeRepoAddCommentLike(false);
            SetupPostRepoGetById(false, true);
            SetupCommentRepoGetById(false);
            SetupFollowRepoGetFollowStatus(false, isInFollows, isAccepted);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task LikeComment_ExistingCommentPrivateUserNullFollow_ReturnsBadRequest()
        {
            SetupLikeRepoAddCommentLike(false);
            SetupPostRepoGetById(false, true);
            SetupCommentRepoGetById(false);
            SetupFollowRepoGetFollowStatus(true, true, true);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task LikeComment_ExistingCommentNullPost_ReturnsBadRequest()
        {
            SetupLikeRepoAddCommentLike(false);
            SetupPostRepoGetById(true, false);
            SetupCommentRepoGetById(false);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task LikeComment_NotExistingComment_ReturnsBadRequest()
        {
            SetupCommentRepoGetById(true);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task LikeComment_ExistingComment_ReturnsBadRequest()
        {
            SetupLikeRepoAddCommentLike(true);
            SetupPostRepoGetById(false, false);
            SetupCommentRepoGetById(false);

            var result = await _controller.LikeComment(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task RemovePostLike_ExistingLike_ReturnsOk()
        {
            SetupLikeRepoRemoveLike(false);
            SetupLikeRepoGetPostLike(false);

            var result = await _controller.RemovePostLike(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task RemovePostLike_NotExistingLike_ReturnsBadRequest()
        {
            SetupLikeRepoRemoveLike(false);
            SetupLikeRepoGetPostLike(true);

            var result = await _controller.RemovePostLike(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task RemovePostLike_Error_ReturnsBadRequest()
        {
            SetupLikeRepoRemoveLike(true);
            SetupLikeRepoGetPostLike(false);

            var result = await _controller.RemovePostLike(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task RemoveCommentLike_ExistingLike_ReturnsOk()
        {
            SetupLikeRepoRemoveLike(false);
            SetupLikeRepoGetCommentLike(false);

            var result = await _controller.RemoveCommentLike(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task RemoveCommentLike_NotExistingLike_ReturnsBadRequest()
        {
            SetupLikeRepoRemoveLike(false);
            SetupLikeRepoGetCommentLike(true);

            var result = await _controller.RemoveCommentLike(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task RemoveCommentLike_Error_ReturnsBadRequest()
        {
            SetupLikeRepoRemoveLike(true);
            SetupLikeRepoGetCommentLike(false);

            var result = await _controller.RemoveCommentLike(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetLikesForPost_ExistingPost_ReturnsOk()
        {
            SetupPostRepoGetById(false, false);
            SetupLikeRepoGetLikesForPost(false);

            var result = await _controller.GetLikesForPost(1, new LikeParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForPost_NotExistingPost_ReturnsBadRequest()
        {
            SetupPostRepoGetById(true, false);
            SetupLikeRepoGetLikesForPost(false);

            var result = await _controller.GetLikesForPost(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForPost_ExistingPostPrivateUser_ReturnsOk()
        {
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(false, true, true);
            SetupLikeRepoGetLikesForPost(false);

            var result = await _controller.GetLikesForPost(1, new LikeParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForPost_ExistingPostPrivateUserNullFollow_ReturnsBadRequest()
        {
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(true, true, true);
            SetupLikeRepoGetLikesForPost(false);

            var result = await _controller.GetLikesForPost(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public async Task GetLikesForPost_ExistingPostPrivateUserBadFollow_ReturnsUnauthorized(bool isInFollows, bool isAccepted)
        {
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(false, isInFollows, isAccepted);
            SetupLikeRepoGetLikesForPost(false);

            var result = await _controller.GetLikesForPost(1, new LikeParameters());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task GetLikesForPost_NullLikes_ReturnsBadRequest()
        {
            SetupPostRepoGetById(false, false);
            SetupLikeRepoGetLikesForPost(true);

            var result = await _controller.GetLikesForPost(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForComment_ExistingComment_ReturnsOk()
        {
            SetupPostRepoGetById(false, false);
            SetupCommentRepoGetById(false);
            SetupLikeRepoGetLikesForComment(false);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForComment_NotExistingComment_ReturnsBadRequest()
        {
            SetupCommentRepoGetById(true);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForComment_NullPost_ReturnsBadRequest()
        {
            SetupCommentRepoGetById(false);
            SetupPostRepoGetById(true, false);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForComment_ExistingCommentPrivateUser_ReturnsOk()
        {
            SetupCommentRepoGetById(false);
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(false, true, true);
            SetupLikeRepoGetLikesForComment(false);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetLikesForComment_ExistingCommentPrivateUserNullFollow_ReturnsBadRequest()
        {
            SetupCommentRepoGetById(false);
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(true, true, true);
            SetupLikeRepoGetLikesForComment(false);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public async Task GetLikesForComment_ExistingCommentPrivateUserBadFollow_ReturnsUnauthorized(bool isInFollows, bool isAccepted)
        {
            SetupCommentRepoGetById(false);
            SetupPostRepoGetById(false, true);
            SetupFollowRepoGetFollowStatus(false, isInFollows, isAccepted);
            SetupLikeRepoGetLikesForComment(false);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Test]
        public async Task GetLikesForComment_NullLikes_ReturnsBadRequest()
        {
            SetupCommentRepoGetById(false);
            SetupPostRepoGetById(false, false);
            SetupLikeRepoGetLikesForComment(true);

            var result = await _controller.GetLikesForComment(1, new LikeParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private void SetupPostRepoGetById(bool shouldReturnNull, bool shouldUserBePrivate)
        {
            _postRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull ? null : new Post
                {
                    UserId = 1,
                    User = new User
                    {
                        IsPrivate = shouldUserBePrivate
                    }
                });
        }

        private void SetupFollowRepoGetFollowStatus(bool shouldReturnNull, bool isInFollows, bool isAccepted)
        {
            _followRepository.Setup(x => x.GetFollowStatus(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull ? null : new FollowStatusDto(isInFollows, isAccepted));
        }

        private void SetupLikeRepoAddPostLike(bool shouldReturnFalse)
        {
            _likeRepository.Setup(x => x.AddPostLike(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupLikeRepoAddCommentLike(bool shouldReturnFalse)
        {
            _likeRepository.Setup(x => x.AddCommentLike(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }

        private void SetupCommentRepoGetById(bool shouldReturnNull)
        {
            _commentRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull ? null : new Comment { PostId = 1 });
        }

        private void SetupLikeRepoGetPostLike(bool shouldReturnNull)
        {
            _likeRepository.Setup(x => x.GetPostLike(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull ? null : new PostLike());
        }
        
        private void SetupLikeRepoGetCommentLike(bool shouldReturnNull)
        {
            _likeRepository.Setup(x => x.GetCommentLike(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(shouldReturnNull ? null : new CommentLike());
        }

        private void SetupLikeRepoRemoveLike(bool shouldReturnFalse)
        {
            _likeRepository.Setup(x => x.RemoveLike(It.IsAny<PostLike>()))
                .ReturnsAsync(!shouldReturnFalse);
            _likeRepository.Setup(x => x.RemoveLike(It.IsAny<CommentLike>()))
                .ReturnsAsync(!shouldReturnFalse);
        }

        private void SetupLikeRepoGetLikesForPost(bool shouldReturnNull)
        {
            _likeRepository.Setup(x => x.GetLikesForPost(It.IsAny<int>(), It.IsAny<LikeParameters>()))
                .ReturnsAsync(shouldReturnNull
                    ? null
                    : new PagedList<PostLikeForListDto>(new List<PostLikeForListDto>(), 1, 1, 1));
        }

        private void SetupLikeRepoGetLikesForComment(bool shouldReturnNull)
        {
            _likeRepository.Setup(x => x.GetLikesForComment(It.IsAny<int>(), It.IsAny<LikeParameters>()))
                .ReturnsAsync(shouldReturnNull
                    ? null
                    : new PagedList<CommentLikeForListDto>(new List<CommentLikeForListDto>(), 1, 1, 1));
        }
    }
}