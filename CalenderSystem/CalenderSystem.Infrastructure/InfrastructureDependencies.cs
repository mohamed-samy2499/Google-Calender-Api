using Microsoft.Extensions.DependencyInjection;
using CalenderSystem.Infrastructure.Generic;
using CalenderSystem.Infrastructure.Repositories.EventRepositories;
using CalenderSystem.Infrastructure.Repositories.ApplicationUserRepositories;
using CalenderSystem.Infrastructure.Repositories.EventRepository;

namespace CalenderSystem.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IEventRepository, EventRepository>();

            services.AddTransient < IApplicationUserRepository, ApplicationUserRepository>();
            return services;
        }
    }
}
