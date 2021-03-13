using FluentAssertions;
using MimeKit;
using MyPics.Domain.Email;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Email
{
    [TestFixture]
    public class EmailMessageTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var message = new EmailMessage
            {
                Sender = new MailboxAddress("testSender", "test@email.com"),
                Receiver = new MailboxAddress("testReceiver", "test1@email.com"),
                Subject = "testSubject",
                Content = "testContent"
            };

            message.Should().NotBeNull();
            message.Sender.Name.Should().Be("testSender");
            message.Sender.Address.Should().Be("test@email.com");
            message.Receiver.Name.Should().Be("testReceiver");
            message.Receiver.Address.Should().Be("test1@email.com");
            message.Subject.Should().Be("testSubject");
            message.Content.Should().Be("testContent");
        }
    }
}