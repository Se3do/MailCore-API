using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailCore.Infrastructure.Data.Seeding
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(MailCoreDbContext context, IPasswordHasher passwordHasher)
        {
            if (await context.Users.AnyAsync())
                return;

            await using var transaction = await context.Database.BeginTransactionAsync();

            var alicePassword = passwordHasher.Hash("alice123");
            var alice = User.Create("Alice Johnson", "alice@mailcore.local", alicePassword);
            var bobPassword = passwordHasher.Hash("bob123");
            var bob = User.Create("Bob Smith", "bob@mailcore.local", bobPassword);

            context.Users.AddRange(alice, bob);

            var aliceWork = Label.Create(alice.Id, "Work", "#3b82f6");
            var alicePersonal = Label.Create(alice.Id, "Personal", "#10b981");
            var bobWork = Label.Create(bob.Id, "Work", "#f59e0b");

            context.Labels.AddRange(aliceWork, alicePersonal, bobWork);

            var thread = Domain.Entities.Thread.Create(
                createdAt: DateTime.UtcNow.AddDays(-2),
                lastMessageAt: DateTime.UtcNow.AddHours(-1));
            context.Threads.Add(thread);

            var email1 = Email.Create(alice.Id, "Welcome to MailCore", "Hi Bob,\n\nWelcome to MailCore! Let me know if you have any questions.\n\nBest,\nAlice", thread.Id,
                createdAt: DateTime.UtcNow.AddDays(-2));
            email1.MarkAsSent(DateTime.UtcNow.AddDays(-2));
            context.Emails.Add(email1);

            var email2 = Email.Create(bob.Id, "Re: Welcome to MailCore", "Thanks Alice! Looks great so far.\n\nBob", thread.Id,
                createdAt: DateTime.UtcNow.AddHours(-1));
            email2.MarkAsSent(DateTime.UtcNow.AddHours(-1));
            context.Emails.Add(email2);

            var mr1 = MailRecipient.Create(bob.Id, email1.Id, RecipientType.To, email1.CreatedAt);
            mr1.MarkAsRead();
            context.MailRecipients.Add(mr1);

            var mr2 = MailRecipient.Create(alice.Id, email2.Id, RecipientType.To, email2.CreatedAt);
            context.MailRecipients.Add(mr2);

            context.MailRecipientLabels.Add(MailRecipientLabel.Create(mr1.Id, bobWork.Id));

            var draft = Draft.Create(alice.Id, "Meeting notes", "Discuss Q2 roadmap and resource allocation.");
            context.Drafts.Add(draft);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
