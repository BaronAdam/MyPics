using System;
using System.Threading.Tasks;
using System.Web;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MyPics.Domain.Email;
using MyPics.Infrastructure.Interfaces;

namespace MyPics.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client;
        private readonly EmailConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        { 
            _configuration = (EmailConfiguration) configuration.GetSection("EmailConfirmation");
            
            _client = new SmtpClient();
            try
            {
                _client.Connect(_configuration.SmtpServer, _configuration.Port, true);
                _client.Authenticate(_configuration.UserName, _configuration.Password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        ~EmailService()
        {
            _client.Disconnect(true);
        }
        
        public async Task<bool> SendEmail(EmailMessage message)
        {
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);
            
            try
            {
                await _client.SendAsync(mimeMessage);
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