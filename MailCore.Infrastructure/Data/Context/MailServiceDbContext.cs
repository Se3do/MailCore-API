using MailCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailCore.Infrastructure.Data.Context
{
    public class MailCoreDbContext : DbContext
    {
        public MailCoreDbContext(DbContextOptions<MailCoreDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Email> Emails => Set<Email>();
        public DbSet<Domain.Entities.Thread> Threads => Set<Domain.Entities.Thread>();
        public DbSet<MailRecipient> MailRecipients => Set<MailRecipient>();
        public DbSet<Label> Labels => Set<Label>();
        public DbSet<MailRecipientLabel> MailRecipientLabels => Set<MailRecipientLabel>();
        public DbSet<Draft> Drafts => Set<Draft>();
        public DbSet<Attachment> Attachments => Set<Attachment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MailCoreDbContext).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
