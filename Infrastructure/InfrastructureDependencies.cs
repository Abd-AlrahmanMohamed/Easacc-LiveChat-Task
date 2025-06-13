using Infrastructure.Implementations;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUnityOfWork, UnityOfWork>();

            return services;
        }
    }
}
