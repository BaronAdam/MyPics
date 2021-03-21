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

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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

        
    }
}