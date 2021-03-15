using System.Threading.Tasks;
using MyPics.Domain.Email;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendEmail(EmailMessage message);
    }
}