using System.Collections.Generic;

namespace MyPics.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string PicturePublicId { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}