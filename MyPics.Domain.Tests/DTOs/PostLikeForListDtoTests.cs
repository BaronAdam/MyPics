using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class PostLikeForListDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new PostLikeForListDto()
            {
                UserId = 1,
                PostId = 1,
                User = new UserForLikeDto()
            };

            entity.Should().NotBeNull();
            entity.UserId.Should().Be(1);
            entity.PostId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new UserForLikeDto());
        }
    }
}