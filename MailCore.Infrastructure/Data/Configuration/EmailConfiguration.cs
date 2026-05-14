using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailCore.Infrastructure.Data.Configuration
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Body)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DeliveryStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue(EmailDeliveryStatus.Pending);

            builder.Property(e => e.SentAt)
                .IsRequired(false);

            builder.Property(e => e.SendAttempts)
                .HasDefaultValue(0);

            builder.Property(e => e.LastSendError)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.HasOne(e => e.Sender)
                .WithMany(u => u.SentEmails)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.ThreadId);
            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.DeliveryStatus);
        }
    }
}
