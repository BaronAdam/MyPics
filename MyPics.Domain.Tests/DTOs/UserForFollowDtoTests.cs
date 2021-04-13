using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class UserForFollowDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new UserForFollowDto
            {
                Id = 1,
                Username = "testUsername1",
                DisplayName = "testDisplayName",
                ProfilePictureUrl = "test.com/pic"
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Username.Should().Be("testUsername1");
            entity.DisplayName.Should().Be("testDisplayName");
            entity.ProfilePictureUrl.Should().Be("test.com/pic");
        }
    }
}