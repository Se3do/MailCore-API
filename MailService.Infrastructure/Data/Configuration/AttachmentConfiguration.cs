using MailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailService.Infrastructure.Data.Configuration
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Email)
            .WithMany(e => e.Attachments)
            .HasForeignKey(a => a.EmailId)
            .OnDelete(DeleteBehavior.Cascade);
        }

    }
    //TODO : Add other entity configurations here
}
