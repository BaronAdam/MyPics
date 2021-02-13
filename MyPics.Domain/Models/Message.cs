namespace MyPics.Domain.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipientId { get; set; }
        public User User { get; set; }
        public User Recipient { get; set; }
        public string Content { get; set; }
        public bool IsPhoto { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
    }
}