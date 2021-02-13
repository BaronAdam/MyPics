using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyPics.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string DisplayName { get; set; }
        [Encrypted] public string Email { get; set; }
        [Encrypted] public string ProfilePictureUrl { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Follow> Following { get; set; }
        public virtual ICollection<Follow> Followers { get; set; }
    }
}