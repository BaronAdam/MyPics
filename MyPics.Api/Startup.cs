using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyPics.Api.Configuration;
using MyPics.Domain.Email;
using MyPics.Infrastructure.Persistence;
using MyPics.Infrastructure.Persistence.DatabaseSeed;

namespace MyPics.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EmailConfiguration>(Configuration.GetSection("EmailConfiguration"));
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            
            services.AddControllers();
            
            services.ConfigureSwagger();

            services.AddDbContext<MyPicsDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddAutoMapper(typeof(Startup).Assembly);
            
            services.ConfigureDependencyInjection();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<MyPicsDbContext>();
                var databaseSeed = new Seed(context);
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                databaseSeed.SeedData();
            }
            
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyPics");
                c.RoutePrefix = "swagger";
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
