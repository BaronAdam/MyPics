using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPics.Domain.DTOs;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;

namespace MyPics.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFollowRepository _followRepository;

        public UserController(IUserRepository userRepository, IMapper mapper, IFollowRepository followRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _followRepository = followRepository;
        }

        [HttpGet("find/{username}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserForSearchDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FindUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest("Please provide a valid Username");

            var user = await _userRepository.GetUserByUsername(username);

            if (user == null) return BadRequest("Not found user with specified username");

            var mappedUser = _mapper.Map<UserForSearchDto>(user);

            return Ok(mappedUser);
        }

        [ProducesResponseType(typeof(PagedList<UserForFollowDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
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
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        [HttpGet("followers/{username}")]
        public async Task<IActionResult> FindUserInFollowers(string username)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

            var user = await _followRepository.FindUserInFollowers(userId, username);

            if (user == null) return BadRequest("Could not find a specified user");

            return Ok(user);
        }
    }
}