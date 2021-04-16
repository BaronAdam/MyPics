using System;
using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class UserForPostDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new UserForPostDto
            {
                Id = 1,
                DisplayName = "testName",
                ProfilePictureUrl = "test.com/pic"
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.DisplayName.Should().Be("testName");
            entity.ProfilePictureUrl.Should().Be("test.com/pic");
        }
    }
}