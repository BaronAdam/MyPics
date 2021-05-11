using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class UserForCommentDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new UserForCommentDto
            {
                Id = 1,
                DisplayName = "testName",
                ProfilePictureUrl = "url.com/test"
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.DisplayName.Should().Be("testName");
            entity.ProfilePictureUrl.Should().Be("url.com/test");
        }
    }
}