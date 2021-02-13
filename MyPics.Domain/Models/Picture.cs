using System.ComponentModel.DataAnnotations;

namespace MyPics.Domain.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        [Encrypted] public string Url { get; set; }
    }
}