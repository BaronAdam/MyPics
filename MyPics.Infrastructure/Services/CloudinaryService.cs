using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Persistence;
using MyPics.Infrastructure.Persistence.Enums;

namespace MyPics.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        private static readonly List<string> ImageFileExtensions =
            new() {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg"};
        private static readonly List<string> VideoFileExtensions = new() {".avi", ".mp4", ".mov", ".mkv", ".wmv"};

        public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            var account = new Account(
                cloudinarySettings.Value.CloudName,
                cloudinarySettings.Value.ApiKey,
                cloudinarySettings.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<CustomUploadResult> UploadFile(Stream stream, string filename)
        {
            var fileType = CheckFileType(filename);

            if (fileType == TypeOfContent.Image) return await UploadImage(stream);

            if (fileType == TypeOfContent.Video) return await UploadVideo(stream);

            return null;
        }

        

        private TypeOfContent CheckFileType(string filename)
        {
            var extension = Path.GetExtension(filename);
            
            if (ImageFileExtensions.Contains(extension.ToLowerInvariant())) return TypeOfContent.Image;
            
            if (VideoFileExtensions.Contains(extension.ToLowerInvariant())) return TypeOfContent.Video;
            
            return TypeOfContent.Other;
        }

        private async Task<CustomUploadResult> UploadImage(Stream stream)
        {
            var uploadParameters = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
                Transformation = new Transformation().Quality(50)
            };
            var uploadResult = await UploadImageAsync(uploadParameters);

            return new CustomUploadResult(uploadResult);
        }
        
        private async Task<CustomUploadResult> UploadVideo(Stream stream)
        {
            var uploadParameters = new VideoUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
                Transformation = new Transformation().Quality(50)
            };
            var uploadResult = await UploadVideoAsync(uploadParameters);

            return new CustomUploadResult(uploadResult);
        }
        
        private async Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams)
        {
            try
            {
                return await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private async Task<VideoUploadResult> UploadVideoAsync(VideoUploadParams uploadParams)
        {
            try
            {
                return await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}