using FluentValidation;
using Messenger.Application.Common.Behaviors;
using Messenger.Application.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messenger.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var applicationAssembly = typeof(DependencyInjection).Assembly;

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            RegisterValidators(builder.Services, applicationAssembly);
        }

        private static void RegisterValidators(IServiceCollection services, System.Reflection.Assembly assembly)
        {
            var validatorInterface = typeof(IValidator<>);

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var implemented = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterface);

                foreach (var serviceType in implemented)
                {
                    services.AddScoped(serviceType, type);
                }
            }
        }
    }
}
