using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class PostForAddDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var dto = new PostForAddDto
            {
                Description = "testDescription",
                NumberOfPictures = 5,
                Files = new List<IFormFile>()
            };

            dto.Should().NotBeNull();
            dto.Description.Should().Be("testDescription");
            dto.NumberOfPictures.Should().Be(5);
            dto.Files.Should().BeEmpty();
        }
    }
}