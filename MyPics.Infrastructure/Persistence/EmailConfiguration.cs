namespace MyPics.Infrastructure.Persistence
{
    public class EmailConfiguration
    {
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}