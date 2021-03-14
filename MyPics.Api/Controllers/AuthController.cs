using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;

namespace MyPics.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration, IMapper mapper, 
            IEmailService emailService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
        }
        
        [HttpPost("register")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
        {
            if (await _authRepository.UserExists(userForRegister.Username)) return BadRequest("Username already exists");
            
            if (await _authRepository.EmailExists(userForRegister.Email)) return BadRequest("Email already registered");

            var user = _mapper.Map<User>(userForRegister);

            var registeredUser = await _authRepository.Register(user, userForRegister.Password);

            if (registeredUser == null) return StatusCode((int) HttpStatusCode.InternalServerError);

            var result = await SendEmail(registeredUser);

            return result ? Ok() : StatusCode((int) HttpStatusCode.InternalServerError);
        }
        
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            var user = await _authRepository.Login(userForLogin.Username, userForLogin.Password);

            if (user == null) return Unauthorized();

            if (!user.IsConfirmed)
            {
                var result = await SendEmail(user);
                
                return result ? Unauthorized("Please confirm your e-mail address first.") 
                    : StatusCode((int) HttpStatusCode.InternalServerError);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            
            SymmetricSecurityKey key;
            
            try
            {
                key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(
                new {
                token = tokenHandler.WriteToken(token)
            });
        }

        [HttpGet("confirmEmail")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ConfirmEmail(string token, string username)
        {
            var result = await _authRepository.ConfirmEmail(token, username);

            return result ? Ok("Email confirmed.") : Unauthorized("Verification link is not valid.");
        }

        private async Task<bool> SendEmail(User user)
        {
            var emailActionLink = Url.Action("ConfirmEmail", "Auth",
                new { Token = user.RegistrationToken, Username = user.Username }, Request.Scheme);

            var message = _emailService.BuildConfirmationMessage(user.Email, user.Username, emailActionLink);
            
            return await _emailService.SendEmail(message);
        }
    }
}