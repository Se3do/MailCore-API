using MailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailService.Infrastructure.Data.Configuration
{
    public class MailRecipientConfiguration : IEntityTypeConfiguration<MailRecipient>
    {
        public void Configure(EntityTypeBuilder<MailRecipient> builder)
        {
            builder.HasKey(mr => mr.Id);

            builder.HasIndex(mr => new { mr.EmailId, mr.UserId })
                .IsUnique();

            builder.HasIndex(mr => new { mr.UserId, mr.ReceivedAt });

            builder.HasOne(mr => mr.User)
                .WithMany(u => u.MailRecipients)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(mr => mr.Email)
                .WithMany(e => e.Recipients)
                .HasForeignKey(mr => mr.EmailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    //TODO : Add other entity configurations here
}
