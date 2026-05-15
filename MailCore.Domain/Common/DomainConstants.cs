namespace MailCore.Domain.Common;

public static class DomainConstants
{
    public const long MaxAttachmentSizeBytes = 10 * 1024 * 1024;
    public const int DefaultDispatchBatchSize = 10;
    public const int MaxSendAttempts = 3;
    public const int DispatchIntervalSeconds = 5;
}
