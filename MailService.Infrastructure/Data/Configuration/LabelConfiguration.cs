using MailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailService.Infrastructure.Data.Configuration
{
    public class LabelConfiguration : IEntityTypeConfiguration<Label>
    {
        public void Configure(EntityTypeBuilder<Label> builder)
        {
            builder.HasKey(l => l.Id);

            builder.HasIndex(l => new { l.UserId, l.Name })
            .IsUnique();

            builder.HasOne(l => l.User)
            .WithMany(u => u.Labels)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
    //TODO : Add other entity configurations here
}
