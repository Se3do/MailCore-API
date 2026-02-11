using MailService.Application.Services;
using MailService.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MailService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDraftService, DraftService>();
            services.AddScoped<IMailboxService, MailboxService>();
            services.AddScoped<ILabelService, LabelService>();

            return services;
        }
    }
}
