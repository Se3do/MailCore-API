using MailCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailCore.Infrastructure.Data.Configuration
{
    public class DraftConfiguration : IEntityTypeConfiguration<Draft>
    {
        public void Configure(EntityTypeBuilder<Draft> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(d => d.Body)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

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
}
