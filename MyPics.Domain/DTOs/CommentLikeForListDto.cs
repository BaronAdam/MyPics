namespace MyPics.Domain.DTOs
{
    public class CommentLikeForListDto
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public UserForLikeDto User { get; set; }
    }
}