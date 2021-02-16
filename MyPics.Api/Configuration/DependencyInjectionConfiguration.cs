using Microsoft.Extensions.DependencyInjection;
using MyPics.Infrastructure.Interfaces;
using MyPics.Infrastructure.Repositories;

namespace MyPics.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}