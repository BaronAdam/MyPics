using FluentAssertions;
using MyPics.Infrastructure.Persistence;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Persistence
{
    [TestFixture]
    public class CloudinarySettingsTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var settings = new CloudinarySettings()
            {
                CloudName = "testCloudName",
                ApiKey = "testApiKey",
                ApiSecret = "testApiSecret"
            };

            settings.Should().NotBeNull();
            settings.CloudName.Should().Be("testCloudName");
            settings.ApiKey.Should().Be("testApiKey");
            settings.ApiSecret.Should().Be("testApiSecret");
        }
    }
}