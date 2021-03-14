using FluentAssertions;
using MyPics.Domain.Email;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Email
{
    [TestFixture]
    public class EmailConfigurationTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var configuration = new EmailConfiguration
            {
                Sender = "test@email.com",
                SenderName = "testName",
                SmtpServer = "test.smtp.com",
                Port = 123,
                UserName = "testUsername",
                Password = "testPassword",
            };

            configuration.Should().NotBeNull();
            configuration.Sender.Should().Be("test@email.com");
            configuration.SenderName.Should().Be("testName");
            configuration.SmtpServer.Should().Be("test.smtp.com");
            configuration.Port.Should().Be(123);
            configuration.UserName.Should().Be("testUsername");
            configuration.Password.Should().Be("testPassword");
        }
    }
}