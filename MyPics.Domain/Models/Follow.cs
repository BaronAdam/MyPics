namespace MyPics.Domain.Models
{
    public class Follow
    {
        public int UserId { get; set; }
        public int FollowingId { get; set; }
        public User User { get; set; }
        public User Following { get; set; }
        public bool IsAccepted { get; set; }
    }
}