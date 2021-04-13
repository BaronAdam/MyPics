using System.ComponentModel.DataAnnotations;

namespace MyPics.Domain.DTOs
{
    public class PostForUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
    }
}