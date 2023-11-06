using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CalenderSystem.Application.Behaviors;
using CalenderSystem.Application.IServices;
using CalenderSystem.Application.Services;
using System.Reflection;
using MovieSystem.Application.Services;

namespace CalenderSystem.Application
{
    public static class ApplicationDependeicies
    {
        public static IServiceCollection AddApplicationDependeicies(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IEventService, EventService>();

            services.AddTransient<IApplicationUserService, ApplicationUserService>();


            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());


            // Get Validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // 
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
