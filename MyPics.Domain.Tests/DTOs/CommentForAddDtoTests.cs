using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class CommentForAddDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new CommentForAddDto
            {
                PostId = 1,
                Content = "testContent",
                IsReply = true,
                ParentCommentId = 2
            };

            entity.Should().NotBeNull();
            entity.PostId.Should().Be(1);
            entity.Content.Should().Be("testContent");
            entity.IsReply.Should().BeTrue();
            entity.ParentCommentId.Should().Be(2);
        }
    }
}