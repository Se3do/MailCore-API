using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailCore.Infrastructure.Data.Configuration
{
    public class ThreadConfiguration : IEntityTypeConfiguration<Domain.Entities.Thread>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Thread> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasMany(t => t.Emails)
                .WithOne(e => e.Thread!)
                .HasForeignKey(e => e.ThreadId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    //TODO : Add other entity configurations here
}
