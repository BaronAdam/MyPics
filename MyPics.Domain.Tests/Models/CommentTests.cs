using System.Collections.Generic;
using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class CommentTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new Comment
            {
                Id = 1,
                PostId = 1,
                Post = new Post(),
                UserId = 1,
                User = new User(),
                Content = "Test Content",
                IsReply = true,
                ParentCommentId = 1,
                Likes = new List<CommentLike>()
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.PostId.Should().Be(1);
            entity.Post.Should().BeEquivalentTo(new Post());
            entity.UserId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Content.Should().Be("Test Content");
            entity.IsReply.Should().BeTrue();
            entity.ParentCommentId.Should().Be(1);
            entity.Likes.Should().BeEmpty();
        }
    }
}