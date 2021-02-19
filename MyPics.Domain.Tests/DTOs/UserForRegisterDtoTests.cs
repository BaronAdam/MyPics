using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class UserForRegisterDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new UserForRegisterDto
            {
                Username = "testUsername",
                Password = "testPassword",
                Email = "email@test.com",
                DisplayName = "Test User"
            };

            entity.Should().NotBeNull();
            entity.Username.Should().Be("testUsername");
            entity.Password.Should().Be("testPassword");
            entity.Email.Should().Be("email@test.com");
            entity.DisplayName.Should().Be("Test User");
        }
    }
}