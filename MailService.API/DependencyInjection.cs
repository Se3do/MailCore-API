using MailService.Infrastructure;
using MailService.Application;

namespace MailService.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            // Application service registrations go here
            services.AddApplicationDI().
                AddInfrastructureDI(configuration);
            return services;
        }
    }
}
