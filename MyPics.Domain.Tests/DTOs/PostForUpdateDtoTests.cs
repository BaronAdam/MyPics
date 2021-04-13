using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class PostForUpdateDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var dto = new PostForUpdateDto
            {
                Id = 1,
                Description = "testDescription"
            };

            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Description.Should().Be("testDescription");
        }
    }
}