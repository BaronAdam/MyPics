using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPics.Domain.DTOs;
using MyPics.Infrastructure.Interfaces;
using static System.Int32;

namespace MyPics.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public UserController(IUserRepository userRepository, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
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

        [HttpPost("profile/picture")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UploadProfilePicture([FromForm] ProfilePictureDto pictureDto)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var file = pictureDto.File;

            if (file == null || file.Length <= 0) return BadRequest("There was an error with the file.");
            
            await using var stream = file.OpenReadStream();
            
            var uploadResult = await _cloudinaryService.UploadFile(stream, file.FileName);

            if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Url))
                return BadRequest("There was an error while uploading Your photo.");

            var result = await _userRepository.ChangeProfilePicture(userId, uploadResult.Url);

            return result ? Ok() : BadRequest("There was an error while processing your request.");
        }
    }
}