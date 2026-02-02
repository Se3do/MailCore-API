using MailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailService.Infrastructure.Data.Configuration
{
    public class DraftConfiguration : IEntityTypeConfiguration<Draft>
    {
        public void Configure(EntityTypeBuilder<Draft> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.User)
            .WithMany(u => u.Drafts)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Thread)
            .WithMany()
            .HasForeignKey(d => d.ThreadId)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }   
    //TODO : Add other entity configurations here
}
