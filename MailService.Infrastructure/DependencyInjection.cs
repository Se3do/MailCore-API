using System;
using System.IO;
using MailService.Application.Interfaces.Persistence;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using MailService.Infrastructure.Repositories;
using MailService.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MailServiceDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IDraftRepository, DraftRepository>();
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<IMailRecipientRepository, MailRecipientRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IThreadRepository, ThreadRepository>();

            // read file storage options from configuration without Binder dependency
            var useProjectDirStr = configuration["FileStorage:UseProjectDirectory"];
            var relativePath = configuration["FileStorage:RelativePath"];
            var rootPathConfig = configuration["FileStorage:RootPath"];

            var fileStorageOptions = new FileStorageOptions
            {
                UseProjectDirectory = string.IsNullOrEmpty(useProjectDirStr) ? true : bool.TryParse(useProjectDirStr, out var b) ? b : true,
                RelativePath = relativePath,
                RootPath = rootPathConfig
            };

            services.AddSingleton<IFileStorage>(_ =>
            {
                string rootPath;

                if (fileStorageOptions.UseProjectDirectory)
                {
                    var contentRoot =
                        AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();

                    rootPath = Path.Combine(
                        contentRoot,
                        fileStorageOptions.RelativePath ?? "App_Data/Attachments");
                }
                else
                {
                    rootPath = fileStorageOptions.RootPath
                        ?? throw new InvalidOperationException(
                            "FileStorage:RootPath must be configured for production");
                }

                return new LocalFileStorage(rootPath);
            });


            return services;
        }
    }
}
