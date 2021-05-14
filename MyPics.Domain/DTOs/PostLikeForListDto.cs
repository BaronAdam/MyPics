namespace MyPics.Domain.DTOs
{
    public class PostLikeForListDto
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public UserForLikeDto User { get; set; }
    }
}