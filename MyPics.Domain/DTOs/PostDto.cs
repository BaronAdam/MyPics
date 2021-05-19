using System;
using System.Collections.Generic;

namespace MyPics.Domain.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DatePosted { get; set; }
        public int NumberOfPictures { get; set; }
        public int NumberOfLikes { get; set; }
        public UserForPostDto User { get; set; }
        public ICollection<PictureForPostDto> Pictures { get; set; }
    }
}