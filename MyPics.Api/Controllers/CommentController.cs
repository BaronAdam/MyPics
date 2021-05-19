using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPics.Api.Extensions;
using MyPics.Domain.DTOs;
using MyPics.Domain.Models;
using MyPics.Infrastructure.Helpers;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using MyPics.Infrastructure.Interfaces;
using static System.Int32;

namespace MyPics.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IFollowRepository _followRepository;
        private readonly IMapper _mapper;

        public CommentController(ICommentRepository commentRepository, IPostRepository postRepository, 
            IFollowRepository followRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _followRepository = followRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CommentPost(CommentForAddDto dto)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            if (dto == null) return BadRequest();
            
            var comment = _mapper.Map<Comment>(dto);

            comment.IsReply = comment.ParentCommentId > 0;

            comment.UserId = userId;

            var result = _mapper.Map<CommentForSingleDto>(await _commentRepository.Add(comment));
            
            return result != null ? Ok(result) : StatusCode((int)HttpStatusCode.InternalServerError);
        }
        
        [HttpPatch]
        [ProducesResponseType(typeof(CommentForSingleDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateComment(CommentForEditDto dto)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var comment = await _commentRepository.GetById(dto.Id);

            if (comment == null) return BadRequest("Comment does not exist.");

            if (comment.IsDeleted) return BadRequest("You cannot edit deleted comment.");

            if (comment.UserId != userId) return Unauthorized();
            
            var result = _mapper.Map<CommentForSingleDto>(await _commentRepository.Update(dto));
            
            return result != null ? Ok(result) : StatusCode((int)HttpStatusCode.InternalServerError);
        }
        
        [HttpDelete("{commentId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var comment = await _commentRepository.GetById(commentId);

            if (comment == null || comment.IsDeleted) return BadRequest("Comment does not exist.");

            if (comment.UserId != userId) return Unauthorized();
            
            var result = await _commentRepository.Remove(commentId);
            
            return result ? Ok() : StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [AllowAnonymous]
        [HttpGet("{postId}")]
        [ProducesResponseType(typeof(PagedList<CommentForListDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCommentsForPost(int postId, [FromQuery] CommentParameters parameters)
        {
            var post = await _postRepository.GetById(postId);

            if (post == null) return BadRequest("Post does not exist.");

            if (post.User.IsPrivate)
            {
                if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                    return Unauthorized();

                if (post.UserId != userId)
                {
                    var follow = await _followRepository.GetFollowStatus(userId, post.UserId);

                    if (!follow.IsAlreadyInFollows || !follow.IsFollowAccepted) return Unauthorized();
                }
            }

            var comments = await _commentRepository.GetCommentsForPost(postId, parameters);
            
            if (comments == null) return StatusCode((int)HttpStatusCode.InternalServerError);

            foreach (var comment in comments.Where(comment => comment.IsDeleted))
            {
                comment.User = new UserForCommentDto
                {
                    DisplayName = "[Removed]"
                };
            }

            foreach (var comment in comments)
            {
                comment.NumberOfLikes = await _commentRepository.GetNumberOfLikesForComment(comment.Id);
            }

            Response.AddPaginationHeader(comments.CurrentPage, comments.PageSize, comments.TotalCount, comments.TotalPages);

            return Ok(comments);
        }
        
        [AllowAnonymous]
        [HttpGet("replies/{commentId}")]
        [ProducesResponseType(typeof(PagedList<CommentForListDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRepliesForComment(int commentId, [FromQuery] CommentParameters parameters)
        {
            var comment = await _commentRepository.GetById(commentId);

            if (comment == null) return BadRequest("Comment does not exist.");
            
            var post = await _postRepository.GetById(comment.PostId);

            if (post == null) return BadRequest("Post does not exist.");

            if (post.User.IsPrivate)
            {
                if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                    return Unauthorized();

                if (post.UserId != userId)
                {
                    var follow = await _followRepository.GetFollowStatus(userId, post.UserId);

                    if (!follow.IsAlreadyInFollows || !follow.IsFollowAccepted) return Unauthorized();
                }
            }

            var comments = await _commentRepository.GetRepliesForComment(commentId, parameters);
            
            if (comments == null) return StatusCode((int)HttpStatusCode.InternalServerError);

            foreach (var commentDto in comments.Where(commentDto => commentDto.IsDeleted))
            {
                commentDto.User = new UserForCommentDto
                {
                    DisplayName = "[Removed]"
                };
            }
            
            foreach (var commentDto in comments)
            {
                commentDto.NumberOfLikes = await _commentRepository.GetNumberOfLikesForComment(comment.Id);
            }
            
            Response.AddPaginationHeader(comments.CurrentPage, comments.PageSize, comments.TotalCount, comments.TotalPages);

            return Ok(comments);
        }
    }
}