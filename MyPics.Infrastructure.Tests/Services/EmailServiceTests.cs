using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using MyPics.Domain.Email;
using MyPics.Infrastructure.Services;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Services
{
    [TestFixture]
    public class EmailServiceTests
    {
        private EmailService _service;
        
        [SetUp]
        public void Setup()
        {
            var smtpClientMock = new Mock<ISmtpClient>();
            smtpClientMock.Setup(c =>
                c.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), new CancellationToken()));
            smtpClientMock.Setup(c => c.Authenticate(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()));
            smtpClientMock.Setup(c => c.SendAsync(It.IsAny<MimeMessage>(), new CancellationToken(), null));
            smtpClientMock.Setup(c => c.Disconnect(It.IsAny<bool>(), new CancellationToken()));

            _service = new EmailService(new EmailConfiguration {SenderName = "testSender", Sender = "email@test.com"},
                smtpClientMock.Object);
        }

        [Test]
        public async Task SendEmail_Successful_ReturnsTrue()
        {
            var result = await _service.SendEmail(new EmailMessage
            {
                Content = "",
                Receiver = new MailboxAddress("testName", "test@email.com"),
                Sender = new MailboxAddress("testName1", "test1@email.com"),
                Subject = ""
            });

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task SendEmail_UnSuccessful_ReturnsFalse()
        {
            var result = await _service.SendEmail(new EmailMessage());

            result.Should().BeFalse();
        }
        
        [Test]
        public async Task SendEmail_Exception_ReturnsFalse()
        {
            _service = new EmailService(new EmailConfiguration {SenderName = "testSender", Sender = "email@test.com"},
                null);
            
            var result = await _service.SendEmail(new EmailMessage
            {
                Content = "",
                Receiver = new MailboxAddress("testName", "test@email.com"),
                Sender = new MailboxAddress("testName1", "test1@email.com"),
                Subject = ""
            });

            result.Should().BeFalse();
        }
    }
}