using MailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Data.Context
{
    public class MailServiceDbContext: DbContext
    {
        public MailServiceDbContext(DbContextOptions<MailServiceDbContext> options) : base(options) {}

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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MailServiceDbContext).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
