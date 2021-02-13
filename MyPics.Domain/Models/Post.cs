using System;
using System.Collections.Generic;

namespace MyPics.Domain.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Description { get; set; }
        public DateTime DatePosted { get; set; }
        public int NumberOfPictures { get; set; }

        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<PostLike> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}