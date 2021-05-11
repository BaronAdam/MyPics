using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class CommentForEditDtoTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new CommentForEditDto
            {
                Id = 1,
                Content = "testContent"
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Content.Should().Be("testContent");
        }
    }
}