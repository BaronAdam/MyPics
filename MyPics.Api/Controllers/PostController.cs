﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Interfaces;
using static System.Int32;

namespace MyPics.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFollowRepository _followRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public PostController(IPostRepository postRepository, IPictureRepository pictureRepository, 
            IUserRepository userRepository, IFollowRepository followRepository, ICloudinaryService cloudinaryService,
            IMapper mapper)
        {
            _postRepository = postRepository;
            _pictureRepository = pictureRepository;
            _userRepository = userRepository;
            _followRepository = followRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddPost([FromForm] PostForAddDto postForAddDto)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var post = _mapper.Map<Post>(postForAddDto);

            var formFiles = postForAddDto.Files.ToList();
            
            if (formFiles.Count > 10 || !formFiles.Any()) return BadRequest("Wrong number of files.");

            if (formFiles.Any(file => file == null || file.Length <= 0))
                return BadRequest("There was an error with the file(s).");

            post.NumberOfPictures = formFiles.Count;
            post.UserId = userId;
            post.DatePosted = DateTime.UtcNow;

            var result = await _postRepository.AddPost(post);

            if (result == null) return BadRequest("There was an error while processing Your request");

            var pictures = new List<Picture>();

            foreach (var file in formFiles)
            {
                await using var stream = file.OpenReadStream();
                
                var uploadResult = await _cloudinaryService.UploadFile(stream, file.FileName);

                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Url))
                {
                    await _postRepository.DeletePost(result.Id, userId);
                    
                    return BadRequest("There was an error while uploading Your photo.");
                }
                
                pictures.Add(new Picture {PostId = result.Id, Url = uploadResult.Url});
            }
            
            var picturesResult = await _pictureRepository.AddPicturesForPost(pictures);

            if (!picturesResult) await _postRepository.DeletePost(result.Id, userId);

            return picturesResult ? Ok() 
                : BadRequest("There was an error while processing Your request.");
        }

        [HttpDelete("{postId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeletePost(int postId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var result = await _postRepository.DeletePost(postId, userId);

            return result ? Ok() : BadRequest("Could not delete the post.");
        }

        [HttpPatch]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdatePost(PostForUpdateDto postForUpdate)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var result = await _postRepository.EditPost(postForUpdate, userId);

            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [AllowAnonymous]
        [HttpGet("{postId}/user/{userId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetSinglePost(int postId, int userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null) return BadRequest("User not found.");
            
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var requestingUserId))
                return Unauthorized();
            
            if (user.IsPrivate && userId != requestingUserId)
            {
                var follow = await _followRepository.GetFollowStatus(requestingUserId, userId);

                if (!follow.IsAlreadyInFollows) return BadRequest($"You're not following {user.DisplayName}.");

                if (!follow.IsFollowAccepted) return BadRequest($"{user.DisplayName} have not accepted Your follow.");
            }

            var result = await _postRepository.GetPostForUser(userId, postId);

            return result != null ? Ok(result) : BadRequest("Could not find specified post.");
        }
    }
}