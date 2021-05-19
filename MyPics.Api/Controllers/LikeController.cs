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
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IFollowRepository _followRepository;
        private readonly ICommentRepository _commentRepository;

        public LikeController(ILikeRepository likeRepository, IPostRepository postRepository, 
            IFollowRepository followRepository, ICommentRepository commentRepository)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _followRepository = followRepository;
            _commentRepository = commentRepository;
        }

        [HttpPost("post/{postId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> LikePost(int postId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var post = await _postRepository.GetById(postId);

            if (post == null) return BadRequest("Could not find specified post.");

            if (post.User.IsPrivate)
            {
                var followStatus = await _followRepository.GetFollowStatus(userId, post.UserId);
                
                if (followStatus == null) return BadRequest("There was an error while processing Your request.");

                if (!followStatus.IsAlreadyInFollows || !followStatus.IsFollowAccepted) return Unauthorized();
            }

            var result = await _likeRepository.AddPostLike(postId, userId);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }
        
        [HttpPost("comment/{commentId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var comment = await _commentRepository.GetById(commentId);

            if (comment == null) return BadRequest("Could not find specified post.");
            
            var post = await _postRepository.GetById(comment.PostId);

            if (post == null) return BadRequest("Could not find specified post.");

            if (post.User.IsPrivate)
            {
                var followStatus = await _followRepository.GetFollowStatus(userId, post.UserId);
                
                if (followStatus == null) return BadRequest("There was an error while processing Your request.");

                if (!followStatus.IsAlreadyInFollows || !followStatus.IsFollowAccepted) return Unauthorized();
            }

            var result = await _likeRepository.AddCommentLike(commentId, userId);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [HttpDelete("post/{postId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RemovePostLike(int postId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var like = await _likeRepository.GetPostLike(userId, postId);

            if (like == null) return BadRequest("Like not found.");

            var result = await _likeRepository.RemoveLike(like);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }
        
        [HttpDelete("comment/{commentId}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RemoveCommentLike(int commentId)
        {
            if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                return Unauthorized();

            var like = await _likeRepository.GetCommentLike(userId, commentId);

            if (like == null) return BadRequest("Like not found.");

            var result = await _likeRepository.RemoveLike(like);
            
            return result ? Ok() : BadRequest("There was an error while processing Your request.");
        }

        [AllowAnonymous]
        [HttpDelete("{postId}")]
        [ProducesResponseType(typeof(PagedList<PostLikeForListDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLikesForPost(int postId, [FromQuery] LikeParameters parameters)
        {
            var post = await _postRepository.GetById(postId);

            if (post == null) return BadRequest("Could not find specified post.");

            if (post.User.IsPrivate)
            {
                if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                    return Unauthorized();
                
                var followStatus = await _followRepository.GetFollowStatus(userId, post.UserId);
                
                if (followStatus == null) return BadRequest("There was an error while processing Your request.");

                if (!followStatus.IsAlreadyInFollows || !followStatus.IsFollowAccepted) return Unauthorized();
            }
            
            var likes = await _likeRepository.GetLikesForPost(postId, parameters);
            
            if (likes == null) return BadRequest("There was an error while processing Your request.");
            
            Response.AddPaginationHeader(likes.CurrentPage, likes.PageSize, likes.TotalCount, likes.TotalPages);

            return Ok(likes);
        }
        
        [AllowAnonymous]
        [HttpDelete("{commentId}")]
        [ProducesResponseType(typeof(PagedList<PostLikeForListDto>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLikesForComment(int commentId, [FromQuery] LikeParameters parameters)
        {
            var comment = await _commentRepository.GetById(commentId);

            if (comment == null) return BadRequest("Could not find specified post.");
            
            var post = await _postRepository.GetById(comment.PostId);

            if (post == null) return BadRequest("Could not find specified post.");

            if (post.User.IsPrivate)
            {
                if (!TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out var userId))
                    return Unauthorized();
                
                var followStatus = await _followRepository.GetFollowStatus(userId, post.UserId);
                
                if (followStatus == null) return BadRequest("There was an error while processing Your request.");

                if (!followStatus.IsAlreadyInFollows || !followStatus.IsFollowAccepted) return Unauthorized();
            }
            
            var likes = await _likeRepository.GetLikesForComment(commentId, parameters);
            
            if (likes == null) return BadRequest("There was an error while processing Your request.");
            
            Response.AddPaginationHeader(likes.CurrentPage, likes.PageSize, likes.TotalCount, likes.TotalPages);

            return Ok(likes);
        }
    }
}