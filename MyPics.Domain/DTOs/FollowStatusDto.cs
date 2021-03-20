namespace MyPics.Domain.DTOs
{
    public class FollowStatusDto
    {
        public FollowStatusDto(bool isInFollows = false, bool isAccepted = false)
        {
            IsAlreadyInFollows = isInFollows;
            IsFollowAccepted = isAccepted;
        }
        
        public bool IsAlreadyInFollows { get; set; }
        public bool IsFollowAccepted { get; set; }
    }
}