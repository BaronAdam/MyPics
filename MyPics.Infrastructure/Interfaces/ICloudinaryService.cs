using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace MyPics.Infrastructure.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams);
        Task<VideoUploadResult> UploadVideoAsync(VideoUploadParams uploadParams);
    }
}