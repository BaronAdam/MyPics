using System;
using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class CommentForListDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new CommentForListDto
            {
                Id = 1,
                Content = "testContent",
                IsReply = true,
                ParentCommentId = 2,
                IsDeleted = true,
                DatePosted = new DateTime(2021, 1, 1),
                User = new UserForCommentDto()
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Content.Should().Be("testContent");
            entity.IsReply.Should().BeTrue();
            entity.ParentCommentId.Should().Be(2);
            entity.IsDeleted.Should().BeTrue();
            entity.DatePosted.Should().Be(new DateTime(2021, 1, 1));
            entity.User.Should().BeEquivalentTo(new UserForCommentDto());
        }
    }
}