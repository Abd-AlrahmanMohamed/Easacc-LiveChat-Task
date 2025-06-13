using Application.MediatorHandler.MediatorCommend;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class MediatorHandlers
    {
        public static void RegisterHandlers(this IServiceCollection services)
        {
            // Registering Mediator Handlers for Event
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SendMessageCommand>());



        }
    }
}
