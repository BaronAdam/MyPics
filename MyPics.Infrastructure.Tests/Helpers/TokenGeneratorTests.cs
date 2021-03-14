using FluentAssertions;
using MyPics.Infrastructure.Helpers;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Helpers
{
    [TestFixture]
    public class TokenGeneratorTests
    {
        [Test]
        public void GenerateToken_DefaultSettings_ExpectedResult()
        {
            var result = TokenGenerator.GenerateToken();

            result.Should().NotBeNull();
            result.Should().HaveLength(32);
        }
        
        [TestCase(50)]
        [TestCase(14)]
        [TestCase(74)]
        public void GenerateToken_CustomSettings_ExpectedResult(int size)
        {
            var result = TokenGenerator.GenerateToken(size);

            result.Should().NotBeNull();
            result.Should().HaveLength(size);
        }
    }
}