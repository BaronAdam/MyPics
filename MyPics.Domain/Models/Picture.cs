namespace MyPics.Domain.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
    }
}