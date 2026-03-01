using MailCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailCore.Infrastructure.Data.Configuration
{
    public class MailRecipientLabelConfiguration : IEntityTypeConfiguration<MailRecipientLabel>
    {
        public void Configure(EntityTypeBuilder<MailRecipientLabel> builder)
        {
            builder.HasKey(x => new { x.MailRecipientId, x.LabelId });

            builder.HasOne(x => x.MailRecipient)
            .WithMany(mr => mr.Labels)
            .HasForeignKey(x => x.MailRecipientId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Label)
            .WithMany(l => l.MailRecipients)
            .HasForeignKey(x => x.LabelId)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
    //TODO : Add other entity configurations here
}
