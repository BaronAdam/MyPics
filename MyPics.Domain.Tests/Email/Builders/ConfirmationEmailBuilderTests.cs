using FluentAssertions;
using MimeKit;
using MyPics.Domain.Email.Builders;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Email.Builders
{
    [TestFixture]
    public class ConfirmationEmailBuilderTests
    {
        [TestCase("test1@email.com", "testUsername1", "test1.url.com")]
        [TestCase("test2@email.com", "testUsername2", "test2.url.com")]
        public void BuildConfirmationMessage_Successful_ReturnsExpected(string receiver, string username, string confirmationUrl)
        {
            var result = ConfirmationEmailBuilder.BuildConfirmationMessage(receiver, username, confirmationUrl);

            result.Should().NotBeNull();
            result.Content.Should().Contain(confirmationUrl);
            result.Receiver.Should().BeEquivalentTo(new MailboxAddress(username, receiver));
        }
    }
}