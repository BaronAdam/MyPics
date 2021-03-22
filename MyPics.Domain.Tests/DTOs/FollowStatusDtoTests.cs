using FluentAssertions;
using MyPics.Domain.DTOs;
using NUnit.Framework;

namespace MyPics.Domain.Tests.DTOs
{
    [TestFixture]
    public class FollowStatusDtoTests
    {
        [Test]
        public void TestAll_EmptyConstructor_ExpectedResult()
        {
            var entity = new FollowStatusDto();

            entity.Should().NotBeNull();
            entity.IsAlreadyInFollows.Should().BeFalse();
            entity.IsFollowAccepted.Should().BeFalse();
        }
        
        [Test]
        public void TestAll_OneParameter_ExpectedResult()
        {
            var entity = new FollowStatusDto(true);

            entity.Should().NotBeNull();
            entity.IsAlreadyInFollows.Should().BeTrue();
            entity.IsFollowAccepted.Should().BeFalse();
        }
        
        [Test]
        public void TestAll_TwoParameters_ExpectedResult()
        {
            var entity = new FollowStatusDto(true, true);

            entity.Should().NotBeNull();
            entity.IsAlreadyInFollows.Should().BeTrue();
            entity.IsFollowAccepted.Should().BeTrue();
        }
    }
}