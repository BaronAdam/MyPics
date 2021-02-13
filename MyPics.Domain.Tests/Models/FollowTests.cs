using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class FollowTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new Follow
            {
                UserId = 1,
                FollowingId = 1,
                User = new User(),
                Following = new User()
            };

            entity.Should().NotBeNull();
            entity.UserId.Should().Be(1);
            entity.FollowingId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Following.Should().BeEquivalentTo(new User());
        }
    }
}