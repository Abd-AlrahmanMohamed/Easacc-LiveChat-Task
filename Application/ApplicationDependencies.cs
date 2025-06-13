//using Application.Extensions;
using Application.Extensions;
using Application.Mediator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
//using System.Reflection;

namespace Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencie(this IServiceCollection services)
        {
            services.AddSignalR();
            services.RegisterHandlers();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatorRequest<,>));

            services.Validators();

            return services;
        }
    }
}
