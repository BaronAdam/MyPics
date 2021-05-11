using System.ComponentModel.DataAnnotations;

namespace MyPics.Domain.DTOs
{
    public class CommentForAddDto
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsReply { get; set; }
        public int ParentCommentId { get; set; }
    }
}