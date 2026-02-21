using MailService.Application.Commands.Drafts.CreateDraft;
using MailService.Application.Common.Behaviors;
using MailService.Application.Emails;
using MailService.Application.Interfaces.Services;
using MailService.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MailService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(CreateDraftCommand).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<EmailComposer>();

            return services;
        }
    }
}