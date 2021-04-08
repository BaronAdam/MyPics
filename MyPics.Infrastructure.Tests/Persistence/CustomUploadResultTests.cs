using System;
using CloudinaryDotNet.Actions;
using FluentAssertions;
using MyPics.Infrastructure.Persistence;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Persistence
{
    [TestFixture]
    public class CustomUploadResultTests
    {
        [Test]
        public void TestAll_ImageResult_ExpectedResult()
        {
            var result = new ImageUploadResult
            {
                Url = new Uri("https://localhost:5001/")
            };

            var entity = new CustomUploadResult(result);

            entity.Should().NotBeNull();
            entity.Url.Should().Be("https://localhost:5001/");
        }
        
        [Test]
        public void TestAll_VideoResult_ExpectedResult()
        {
            var result = new VideoUploadResult
            {
                Url = new Uri("https://localhost:5001/")
            };

            var entity = new CustomUploadResult(result);

            entity.Should().NotBeNull();
            entity.Url.Should().Be("https://localhost:5001/");
        }
    }
}