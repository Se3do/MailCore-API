namespace MailCore.Infrastructure.Configuration;

public sealed class EmailOptions
{
    public int DispatchBatchSize { get; set; } = Domain.Common.DomainConstants.DefaultDispatchBatchSize;
    public int MaxSendAttempts { get; set; } = Domain.Common.DomainConstants.MaxSendAttempts;
    public int DispatchIntervalSeconds { get; set; } = Domain.Common.DomainConstants.DispatchIntervalSeconds;
}
