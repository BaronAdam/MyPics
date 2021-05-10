using System;
using System.Collections.Generic;
using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class PostDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new PostDto
            {
                Id = 1,
                Description = "testDescription",
                DatePosted = new DateTime(2021, 01, 01),
                NumberOfPictures = 1,
                User = new UserForPostDto(),
                Pictures = new List<PictureForPostDto>()
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Description.Should().Be("testDescription");
            entity.DatePosted.Should().Be(new DateTime(2021, 01, 01));
            entity.NumberOfPictures.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new UserForPostDto());
            entity.Pictures.Should().BeEmpty();
        }
    }
}