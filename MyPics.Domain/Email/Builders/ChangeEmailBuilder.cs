using MimeKit;

namespace MyPics.Domain.Email.Builders
{
    public class ChangeEmailBuilder
    {
        public static EmailMessage BuildConfirmationMessage(string receiver, string username, string confirmationUrl)
        {
            var messageBody = "Please confirm your email address change by clicking this <a href=\"" 
                              + confirmationUrl + "\">link</a>. Please note the link is valid for 3 hours.<br/>";
            messageBody += "Or copy the following link and paste it in address bar in your browser: <br/>" + confirmationUrl;
            
            return new EmailMessage
            {
                Receiver = new MailboxAddress(username, receiver),
                Subject = "My Pics e-mail confirmation",
                Content = messageBody
            };
        }
    }
}