using System;
using System.Collections.Generic;
using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class PostTests
    {
        private readonly DateTime _testDate = new DateTime(2021, 02, 13);
        
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new Post
            {
                Id = 1,
                UserId = 1,
                User = new User(),
                Description = "Test Description",
                DatePosted = _testDate,
                NumberOfPictures = 1,
                Pictures = new List<Picture>(),
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.UserId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Description.Should().Be("Test Description");
            entity.DatePosted.Should().Be(_testDate);
            entity.NumberOfPictures.Should().Be(1);
            entity.Pictures.Should().BeEmpty();
            entity.Likes.Should().BeEmpty();
            entity.Comments.Should().BeEmpty();
        }
    }
}