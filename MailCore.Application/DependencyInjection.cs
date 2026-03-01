using MailCore.Application.Commands.Drafts.CreateDraft;
using MailCore.Application.Common.Behaviors;
using MailCore.Application.Emails;
using MailCore.Application.Interfaces.Services;
using MailCore.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MailCore.Application
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