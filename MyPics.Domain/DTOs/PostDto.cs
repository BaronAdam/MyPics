using System;

namespace MyPics.Domain.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DatePosted { get; set; }
        public int NumberOfPictures { get; set; }
        public UserForPostDto User { get; set; }
    }
}