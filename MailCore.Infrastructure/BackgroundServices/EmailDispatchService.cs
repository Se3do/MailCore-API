using MailCore.Application.Interfaces.Services;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MailCore.Infrastructure.BackgroundServices
{
    public sealed class EmailDispatchService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailDispatchService> _logger;

        public EmailDispatchService(IServiceScopeFactory scopeFactory, ILogger<EmailDispatchService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
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

                    var pending = await emailRepo.GetPendingAsync(10, stoppingToken);

                    foreach (var email in pending)
                    {
                        try
                        {
                            var recipients = await mrRepo.GetByEmailIdAsync(email.Id, stoppingToken);
                            await sender.SendAsync(email, recipients, stoppingToken);
                            email.DeliveryStatus = EmailDeliveryStatus.Sent;
                            email.SentAt = DateTime.UtcNow;
                            _logger.LogInformation("Email {EmailId} sent successfully", email.Id);
                        }
                        catch (Exception ex)
                        {
                            email.SendAttempts++;
                            email.LastSendError = ex.Message;
                            _logger.LogWarning(ex, "Failed to send email {EmailId} (attempt {Attempt})", email.Id, email.SendAttempts);

                            if (email.SendAttempts >= 3)
                            {
                                email.DeliveryStatus = EmailDeliveryStatus.Failed;
                                _logger.LogError("Email {EmailId} permanently failed after {Attempts} attempts", email.Id, email.SendAttempts);
                            }
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

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Email dispatch service stopped");
        }
    }
}
