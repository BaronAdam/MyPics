using CloudinaryDotNet.Actions;

namespace MyPics.Infrastructure.Persistence
{
    public class CustomUploadResult
    {
        public CustomUploadResult(ImageUploadResult result)
        {
            Url = result.Url.ToString();
        }

        public CustomUploadResult(VideoUploadResult result)
        {
            Url = result.Url.ToString();
        }
        
        public string Url { get; }
    }
}