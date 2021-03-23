using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
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

        public EmailService(IOptions<EmailConfiguration> configuration, ISmtpClient client)
        {
            _configuration = configuration.Value;
            
            _client = client;
        }

        public async Task<bool> SendEmail(EmailMessage message)
        {
            message.Sender = new MailboxAddress(_configuration.SenderName, _configuration.Sender);
            
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);

            if (mimeMessage == null) return false;
            
            try
            {
                await _client.ConnectAsync(_configuration.SmtpServer, _configuration.Port, true);
                await _client.AuthenticateAsync(_configuration.UserName, _configuration.Password);
                await _client.SendAsync(mimeMessage);
                await _client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
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