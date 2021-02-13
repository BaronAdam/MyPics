using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class PostLikeTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new PostLike
            {
                UserId = 1,
                PostId = 1,
                User = new User(),
                Post = new Post()
            };

            entity.Should().NotBeNull();
            entity.PostId.Should().Be(1);
            entity.UserId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Post.Should().BeEquivalentTo(new Post());
        }
    }
}