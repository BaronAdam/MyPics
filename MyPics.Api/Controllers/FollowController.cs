using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPics.Api.Extensions;
using MyPics.Domain.DTOs;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;
using static System.Int32;

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var users = await _followRepository.GetUserFollows(userId, parameters);

            if (users == null || !users.Any()) return BadRequest("Could not find any follows");
            
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
        
        [ProducesResponseType(typeof(PagedList<UserForFollowDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowersForUser([FromQuery] UserParameters parameters)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var users = await _followRepository.GetUserFollowers(userId, parameters);

            if (users == null || !users.Any()) return BadRequest("Could not find any followers");
            
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [ProducesResponseType(typeof(UserForFollowDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("follows/{username}")]
        public async Task<IActionResult> FindUserInFollows(string username)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var result = await _followRepository.FollowUser(userId, followeeId);

            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [ProducesResponseType(typeof(FollowStatusDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("status/{followeeId}")]
        public async Task<IActionResult> GetFollowStatus(int followeeId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

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
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var result = await _followRepository.GetNotAcceptedFollows(userParameters, userId);
            
            if (result == null) return BadRequest("There was an error while processing Your request.");
            
            Response.AddPaginationHeader(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);

            return Ok(result);
        }
    }
}