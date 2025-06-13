using Application.MediatorHandler.MediatorCommend;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class AddValidators
    {
        public static void Validators(this IServiceCollection services)
        {
            //Event
            services.AddValidatorsFromAssembly(typeof(SendMessageCommand).Assembly);
            //services.AddValidatorsFromAssembly(typeof(UpdateEventCommand).Assembly);
            //services.AddValidatorsFromAssembly(typeof(DeleteEventCommand).Assembly);




        }

    }
}
