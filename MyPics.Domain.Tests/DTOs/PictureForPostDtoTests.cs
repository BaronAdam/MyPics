using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class PictureForPostDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new PictureForPostDto
            {
                Id = 1,
                Url = "url.test.com"
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Url.Should().Be("url.test.com");
        }
    }
}