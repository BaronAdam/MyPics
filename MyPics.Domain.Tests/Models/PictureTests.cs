using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class PictureTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new Picture
            {
                Id = 1,
                PostId = 1,
                Post = new Post(),
                Url = "test.com/test"
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.PostId.Should().Be(1);
            entity.Post.Should().BeEquivalentTo(new Post());
            entity.Url.Should().Be("test.com/test");
        }
    }
}