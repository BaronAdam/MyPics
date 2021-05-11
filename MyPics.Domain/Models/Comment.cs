using System;
using System.Collections.Generic;

namespace MyPics.Domain.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public bool IsReply { get; set; }
        public int ParentCommentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DatePosted { get; set; }

        public virtual ICollection<CommentLike> Likes { get; set; }
    }
}