using MyPics.Domain.Models;
using NUnit.Framework;
using FluentAssertions;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class CommentLikeTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new CommentLike
            {
                UserId = 1,
                CommentId = 1,
                User = new User(),
                Comment = new Comment()
            };

            entity.Should().NotBeNull();
            entity.UserId.Should().Be(1);
            entity.CommentId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Comment.Should().BeEquivalentTo(new Comment());
        }
    }
}