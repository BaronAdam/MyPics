using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyPics.Domain.DTOs
{
    public class PostForAddDto
    {
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, 10)]
        public int NumberOfPictures { get; set; }
        [Required]
        public IEnumerable<IFormFile> Files { get; set; }
    }
}