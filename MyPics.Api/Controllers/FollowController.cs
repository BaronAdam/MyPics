﻿using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPics.Domain.DTOs;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;

namespace MyPics.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/user")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowRepository _followRepository;

        public FollowController(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
        }
        
        [ProducesResponseType(typeof(PagedList<UserForFollowDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("follows")]
        public async Task<IActionResult> GetFollowsForUser([FromQuery] UserParameters parameters)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var users = await _followRepository.GetUserFollows(userId, parameters);

            if (users == null || !users.Any()) return BadRequest("Could not find any follows");

            return Ok(users);
        }
        
        [ProducesResponseType(typeof(PagedList<UserForFollowDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowersForUser([FromQuery] UserParameters parameters)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var users = await _followRepository.GetUserFollowers(userId, parameters);

            if (users == null || !users.Any()) return BadRequest("Could not find any followers");

            return Ok(users);
        }

        [ProducesResponseType(typeof(UserForFollowDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("follows/{username}")]
        public async Task<IActionResult> FindUserInFollows(string username)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var user = await _followRepository.FindUserInFollows(userId, username);

            if (user == null) return BadRequest("Could not find a specified user");

            return Ok(user);
        }
        
        [ProducesResponseType(typeof(UserForFollowDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("followers/{username}")]
        public async Task<IActionResult> FindUserInFollowers(string username)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var user = await _followRepository.FindUserInFollowers(userId, username);

            if (user == null) return BadRequest("Could not find a specified user");

            return Ok(user);
        }

        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> FollowUser(int followeeId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.FollowUser(userId, followeeId);

            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [ProducesResponseType(typeof(FollowStatusDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("status/{followeeId}")]
        public async Task<IActionResult> GetFollowStatus(int followeeId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.GetFollowStatus(userId, followeeId);

            return Ok(result);
        }

        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpDelete]
        public async Task<IActionResult> UnFollowUser(int followeeId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.UnFollowUser(userId, followeeId);

            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpPatch]
        public async Task<IActionResult> AcceptFollowRequest(int followerId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.AcceptFollow(userId, followerId);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }
        
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpDelete("reject")]
        public async Task<IActionResult> RejectFollowRequest(int followerId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.RejectFollow(userId, followerId);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }
        
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFollower(int followerId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.RemoveFollower(userId, followerId);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("followers/pending")]
        public async Task<IActionResult> GetNotAcceptedFollowers([FromQuery] UserParameters userParameters)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var result = await _followRepository.GetNotAcceptedFollows(userParameters, userId);
            
            return result != null ? Ok(result) : BadRequest("There was an error while processing Your request.");
        }
    }
}