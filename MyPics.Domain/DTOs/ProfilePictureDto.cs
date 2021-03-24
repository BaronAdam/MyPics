using Microsoft.AspNetCore.Http;

namespace MyPics.Domain.DTOs
{
    public class ProfilePictureDto
    {
        public IFormFile File { get; set; }
    }
}