using MailService.Application.Interfaces.Persistence;
using MailService.Infrastructure.Data.Context;
using MailService.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Repositories;

namespace MailService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MailServiceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IDraftRepository, DraftRepository>();
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<IMailRecipientRepository, MailRecipientRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IThreadRepository, ThreadRepository>();

            var rootPath = configuration["FileStorage:RootPath"]
                ?? Path.Combine(AppContext.BaseDirectory, "Attachments");

            services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(rootPath));

            return services;
        }
    }
}
