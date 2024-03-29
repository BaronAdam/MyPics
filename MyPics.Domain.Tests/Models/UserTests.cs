using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new User
            {
                Id = 1,
                Username = "TestUsername",
                PasswordHash = Encoding.Default.GetBytes("TestPasswordHash"),
                PasswordSalt = Encoding.Default.GetBytes("TestPasswordSalt"),
                IsConfirmed = true,
                RegistrationToken = "testToken",
                RegistrationTokenGeneratedTime = new DateTime(2021, 3, 1),
                DisplayName = "TestDisplayName",
                Email = "email@test.com",
                ProfilePictureUrl = "test.com/test",
                IsPrivate = true,
                Posts = new List<Post>(),
                Messages = new List<Message>(),
                Following = new List<Follow>(),
                Followers = new List<Follow>(),
                CommentLikes = new List<CommentLike>(),
                PostLikes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Username.Should().Be("TestUsername");
            entity.PasswordHash.Should().BeEquivalentTo(Encoding.Default.GetBytes("TestPasswordHash"));
            entity.PasswordSalt.Should().BeEquivalentTo(Encoding.Default.GetBytes("TestPasswordSalt"));
            entity.IsConfirmed.Should().BeTrue();
            entity.RegistrationToken.Should().Be("testToken");
            entity.RegistrationTokenGeneratedTime.Should().Be(new DateTime(2021, 3, 1));
            entity.DisplayName.Should().Be("TestDisplayName");
            entity.Email.Should().Be("email@test.com");
            entity.ProfilePictureUrl.Should().Be("test.com/test");
            entity.IsPrivate.Should().BeTrue();
            entity.Posts.Should().BeEmpty();
            entity.Messages.Should().BeEmpty();
            entity.Following.Should().BeEmpty();
            entity.Followers.Should().BeEmpty();
            entity.CommentLikes.Should().BeEmpty();
            entity.PostLikes.Should().BeEmpty();
            entity.Comments.Should().BeEmpty();
        }
    }
}