using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MyPics.Domain.Email;
using MyPics.Infrastructure.Interfaces;

namespace MyPics.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpClient _client;
        private readonly EmailConfiguration _configuration;

        public EmailService(EmailConfiguration configuration, ISmtpClient client)
        {
            _configuration = configuration;
            
            _client = client;
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
            try
            {
                _client.Disconnect(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public async Task<bool> SendEmail(EmailMessage message)
        {
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);

            if (mimeMessage == null) return false;
            
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
                              + confirmationUrl + "\">link</a>. Please note the link is valid for 3 hours.<br/>";
            messageBody += "Or copy the following link and paste it in address bar in your browser: <br/>" + confirmationUrl;
            
            return new EmailMessage
            {
                Sender = new MailboxAddress(_configuration.SenderName, _configuration.Sender),
                Receiver = new MailboxAddress(username, receiver),
                Subject = "My Pics e-mail confirmation",
                Content = messageBody
            };
        }
        
        private static MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            try
            {
                return new ()
                {
                    From = {message.Sender},
                    To = {message.Receiver},
                    Subject = message.Subject,
                    Body = new TextPart(TextFormat.Html) {Text = message.Content}
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}