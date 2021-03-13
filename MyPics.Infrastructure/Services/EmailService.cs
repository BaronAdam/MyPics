using System;
using System.Threading.Tasks;
using System.Web;
using MailKit.Net.Smtp;
using MimeKit;
using MyPics.Domain.Email;
using MyPics.Infrastructure.Interfaces;

namespace MyPics.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _configuration;

        public EmailService(EmailConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<bool> SendEmail(EmailMessage message)
        {
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);
            
            try
            {
                var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_configuration.SmtpServer, _configuration.Port, true);
                await smtpClient.AuthenticateAsync(_configuration.UserName, _configuration.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public EmailMessage BuildConfirmationMessage(string receiver, string username, string confirmationUrl)
        {
            var messageBody = "Please confirm your account by clicking this <a href=\"" 
                              + confirmationUrl + "\">link</a><br/>";
            messageBody += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser: " 
                                                  + confirmationUrl);
            
            return new EmailMessage
            {
                Sender = new MailboxAddress(_configuration.SenderName, _configuration.Sender),
                Receiver = new MailboxAddress(username, receiver),
                Subject = "My Pics e-mail confirmation",
                Content = messageBody
            };
        }
        
        private MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            return new ()
            {
                From = {message.Sender},
                To = {message.Receiver},
                Subject = message.Subject,
                Body = new TextPart(MimeKit.Text.TextFormat.Html) {Text = message.Content}
            };
        }
    }
}