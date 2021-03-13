using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class UserForLoginDtoTest
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new UserForLoginDto
            {
                Username = "testUsername",
                Password = "testPassword"
            };

            entity.Should().NotBeNull();
            entity.Username.Should().Be("testUsername");
            entity.Password.Should().Be("testPassword");
        }
    }
}