using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class CommentLikeForListDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new CommentLikeForListDto
            {
                UserId = 1,
                CommentId = 1,
                User = new UserForLikeDto()
            };

            entity.Should().NotBeNull();
            entity.UserId.Should().Be(1);
            entity.CommentId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new UserForLikeDto());
        }
    }
}