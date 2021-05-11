using System;

namespace MyPics.Domain.DTOs
{
    public class CommentForSingleDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsReply { get; set; }
        public int ParentCommentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DatePosted { get; set; }
        public UserForCommentDto User { get; set; }
    }
}