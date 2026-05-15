using MailCore.Application.Interfaces.Services;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MailCore.Infrastructure.BackgroundServices
{
    public sealed class EmailDispatchService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailDispatchService> _logger;
        private readonly EmailOptions _options;

        public EmailDispatchService(IServiceScopeFactory scopeFactory, ILogger<EmailDispatchService> logger, IOptions<EmailOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email dispatch service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var emailRepo = scope.ServiceProvider.GetRequiredService<IEmailRepository>();
                    var mrRepo = scope.ServiceProvider.GetRequiredService<IMailRecipientRepository>();
                    var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var pending = await emailRepo.GetPendingAsync(_options.DispatchBatchSize, stoppingToken);

                    foreach (var email in pending)
                    {
                        try
                        {
                            var recipients = await mrRepo.GetByEmailIdAsync(email.Id, stoppingToken);
                            await sender.SendAsync(email, recipients, stoppingToken);
                            email.MarkAsSent();
                            _logger.LogInformation("Email {EmailId} sent successfully", email.Id);
                        }
                        catch (Exception ex)
                        {
                            email.MarkAsFailed(ex.Message);
                            _logger.LogWarning(ex, "Failed to send email {EmailId} (attempt {Attempt})", email.Id, email.SendAttempts);

                            if (email.SendAttempts >= _options.MaxSendAttempts)
                                _logger.LogError("Email {EmailId} permanently failed after {Attempts} attempts", email.Id, email.SendAttempts);
                        }
                    }

                    if (pending.Count > 0)
                    {
                        await uow.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in email dispatch loop");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.DispatchIntervalSeconds), stoppingToken);
            }

            _logger.LogInformation("Email dispatch service stopped");
        }
    }
}
