using MailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailService.Infrastructure.Data.Configuration
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.HasKey(e => e.Id);


            builder.HasOne(e => e.Sender)
            .WithMany(u => u.SentEmails)
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(e => e.ThreadId);
            builder.HasIndex(e => e.CreatedAt);
        }
    }

    //TODO : Add other entity configurations here
}
