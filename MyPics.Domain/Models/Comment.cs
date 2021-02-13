using System.Collections.Generic;

namespace MyPics.Domain.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Content { get; set; }
        public bool IsReply { get; set; }
        public int ParentCommentId { get; set; }
        
        public virtual ICollection<CommentLike> Likes { get; set; }
    }
}