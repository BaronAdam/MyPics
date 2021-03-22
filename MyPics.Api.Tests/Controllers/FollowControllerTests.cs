using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyPics.Api.Controllers;
using MyPics.Domain.DTOs;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;
using NUnit.Framework;

namespace MyPics.Api.Tests.Controllers
{
    [TestFixture]
    public class FollowControllerTests
    {
        private Mock<IFollowRepository> _followRepository;
        private FollowController _controller;
        
        [SetUp]
        public void Setup()
        {
            _followRepository = new Mock<IFollowRepository>();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "123"),
            },"TestAuthentication"));

            _controller = new FollowController(_followRepository.Object)
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

        [Test]
        public async Task FollowUser_Successful_ReturnsOk()
        {
            SetupRepoFollowUser(false);

            var result = await _controller.FollowUser(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task FollowUser_UnSuccessfulOrException_ReturnsBadRequest()
        {
            SetupRepoFollowUser(true);

            var result = await _controller.FollowUser(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public async Task GetFollowStatus_Successful_ReturnsOk(bool isExisting, bool isAccepted)
        {
            SetupRepoGetFollowStatus(isExisting, isAccepted);

            var result = await _controller.GetFollowStatus(1);

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task UnFollowUser_Successful_ReturnsOk()
        {
            SetupRepoUnFollowUser(false);

            var result = await _controller.UnFollowUser(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task UnFollowUser_UnSuccessfulOrException_ReturnsBadRequest()
        {
            SetupRepoUnFollowUser(true);

            var result = await _controller.UnFollowUser(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task AcceptFollowRequest_Successful_ReturnsOk()
        {
            SetupRepoAcceptFollow(false);

            var result = await _controller.AcceptFollowRequest(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task AcceptFollowRequest_UnSuccessfulOrException_ReturnsBadRequest()
        {
            SetupRepoAcceptFollow(true);

            var result = await _controller.AcceptFollowRequest(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task RejectFollowRequest_Successful_ReturnsOk()
        {
            SetupRepoRejectFollow(false);

            var result = await _controller.RejectFollowRequest(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task RejectFollowRequest_UnSuccessfulOrException_ReturnsBadRequest()
        {
            SetupRepoRejectFollow(true);

            var result = await _controller.RejectFollowRequest(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [Test]
        public async Task DeleteFollower_Successful_ReturnsOk()
        {
            SetupRepoRemoveFollower(false);

            var result = await _controller.DeleteFollower(1);

            result.Should().BeOfType<OkResult>();
        }
        
        [Test]
        public async Task DeleteFollower_UnSuccessfulOrException_ReturnsBadRequest()
        {
            SetupRepoRemoveFollower(true);

            var result = await _controller.DeleteFollower(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
        
        [TestCase(false)]
        [TestCase(true)]
        public async Task GetNotAcceptedFollowers_Successful_ReturnsOk(bool shouldReturnEmpty)
        {
            SetupRepoGetNotAcceptedFollows(false, shouldReturnEmpty);

            var result = await _controller.GetNotAcceptedFollowers(new UserParameters());

            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Test]
        public async Task GetNotAcceptedFollowers_UnSuccessfulOrException_ReturnsBadRequest()
        {
            SetupRepoGetNotAcceptedFollows(true, false);

            var result = await _controller.GetNotAcceptedFollowers(new UserParameters());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private void SetupRepoGetUserFollows(bool shouldReturnNull, bool shouldReturnEmpty)
        {
            if (shouldReturnEmpty)
            {
                _followRepository.Setup(x => x.GetUserFollows(It.IsAny<int>(), It.IsAny<UserParameters>()))
                    .ReturnsAsync(() => new PagedList<UserForFollowDto>(new List<UserForFollowDto>(), 0, 0, 0));
                return;
            }

            var list = new List<UserForFollowDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new UserForFollowDto{ Username = "test"});
            }

            _followRepository.Setup(x => x.GetUserFollows(It.IsAny<int>(), It.IsAny<UserParameters>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new PagedList<UserForFollowDto>(list, 1, 10, 10));
        }
        
        private void SetupRepoGetUserFollowers(bool shouldReturnNull, bool shouldReturnEmpty)
        {
            if (shouldReturnEmpty)
            {
                _followRepository.Setup(x => x.GetUserFollowers(It.IsAny<int>(), It.IsAny<UserParameters>()))
                    .ReturnsAsync(() => new PagedList<UserForFollowDto>(new List<UserForFollowDto>(), 0, 0, 0));
                return;
            }

            var list = new List<UserForFollowDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new UserForFollowDto{ Username = "test"});
            }

            _followRepository.Setup(x => x.GetUserFollowers(It.IsAny<int>(), It.IsAny<UserParameters>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new PagedList<UserForFollowDto>(list, 1, 10, 10));
        }
        
        private void SetupRepoFindUserInFollows(bool shouldReturnNull)
        {
            _followRepository.Setup(x => x.FindUserInFollows(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new UserForFollowDto());
        }
        
        private void SetupRepoFindUserInFollowers(bool shouldReturnNull)
        {
            _followRepository.Setup(x => x.FindUserInFollowers(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new UserForFollowDto());
        }

        private void SetupRepoFollowUser(bool shouldReturnFalse)
        {
            _followRepository.Setup(x => x.FollowUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }

        private void SetupRepoGetFollowStatus(bool isExisting, bool isAccepted)
        {
            _followRepository.Setup(x => x.GetFollowStatus(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new FollowStatusDto(isExisting, isAccepted));
        }

        private void SetupRepoUnFollowUser(bool shouldReturnFalse)
        {
            _followRepository.Setup(x => x.UnFollowUser(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }

        private void SetupRepoAcceptFollow(bool shouldReturnFalse)
        {
            _followRepository.Setup(x => x.AcceptFollow(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }

        private void SetupRepoRejectFollow(bool shouldReturnFalse)
        {
            _followRepository.Setup(x => x.RejectFollow(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupRepoRemoveFollower(bool shouldReturnFalse)
        {
            _followRepository.Setup(x => x.RemoveFollower(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(!shouldReturnFalse);
        }
        
        private void SetupRepoGetNotAcceptedFollows(bool shouldReturnNull, bool shouldReturnEmpty)
        {
            if (shouldReturnEmpty)
            {
                _followRepository.Setup(x => x.GetNotAcceptedFollows(It.IsAny<UserParameters>(), It.IsAny<int>()))
                    .ReturnsAsync(() => new PagedList<UserForFollowDto>(new List<UserForFollowDto>(), 0, 0, 0));
                return;
            }

            var list = new List<UserForFollowDto>();

            for (var i = 0; i < 10; i++)
            {
                list.Add(new UserForFollowDto{ Username = "test"});
            }

            _followRepository.Setup(x => x.GetNotAcceptedFollows(It.IsAny<UserParameters>(), It.IsAny<int>()))
                .ReturnsAsync(() => shouldReturnNull ? null : new PagedList<UserForFollowDto>(list, 1, 10, 10));
        }
    }
}