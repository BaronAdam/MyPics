using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Repositories;
using MyPics.Infrastructure.Services;

namespace MyPics.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPictureRepository, PictureRepository>();

            services.AddScoped<ISmtpClient, SmtpClient>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
        }
    }
}