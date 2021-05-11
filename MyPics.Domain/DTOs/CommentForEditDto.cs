using System.ComponentModel.DataAnnotations;

namespace MyPics.Domain.DTOs
{
    public class CommentForEditDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
    }
}