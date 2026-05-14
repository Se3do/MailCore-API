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

            var alicePassword = passwordHasher.Hash("alice123");
            var alice = new User
            {
                Id = Guid.NewGuid(),
                Name = "Alice Johnson",
                Email = "alice@mailcore.local",
                PasswordHash = alicePassword,
                CreatedAt = DateTime.UtcNow
            };

            var bobPassword = passwordHasher.Hash("bob123");
            var bob = new User
            {
                Id = Guid.NewGuid(),
                Name = "Bob Smith",
                Email = "bob@mailcore.local",
                PasswordHash = bobPassword,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(alice, bob);

            var aliceWork = new Label { Id = Guid.NewGuid(), UserId = alice.Id, Name = "Work", Color = "#3b82f6" };
            var alicePersonal = new Label { Id = Guid.NewGuid(), UserId = alice.Id, Name = "Personal", Color = "#10b981" };
            var bobWork = new Label { Id = Guid.NewGuid(), UserId = bob.Id, Name = "Work", Color = "#f59e0b" };

            context.Labels.AddRange(aliceWork, alicePersonal, bobWork);

            var thread = new Domain.Entities.Thread
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                LastMessageAt = DateTime.UtcNow.AddHours(-1)
            };
            context.Threads.Add(thread);

            var email1 = new Email
            {
                Id = Guid.NewGuid(),
                ThreadId = thread.Id,
                SenderId = alice.Id,
                Subject = "Welcome to MailCore",
                Body = "Hi Bob,\n\nWelcome to MailCore! Let me know if you have any questions.\n\nBest,\nAlice",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                HasAttachments = false,
                DeliveryStatus = EmailDeliveryStatus.Sent,
                SentAt = DateTime.UtcNow.AddDays(-2)
            };
            context.Emails.Add(email1);

            var email2 = new Email
            {
                Id = Guid.NewGuid(),
                ThreadId = thread.Id,
                SenderId = bob.Id,
                Subject = "Re: Welcome to MailCore",
                Body = "Thanks Alice! Looks great so far.\n\nBob",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                HasAttachments = false,
                DeliveryStatus = EmailDeliveryStatus.Sent,
                SentAt = DateTime.UtcNow.AddHours(-1)
            };
            context.Emails.Add(email2);

            var mr1 = new MailRecipient
            {
                Id = Guid.NewGuid(),
                UserId = bob.Id,
                EmailId = email1.Id,
                Type = RecipientType.To,
                ReceivedAt = email1.CreatedAt,
                IsRead = true
            };
            context.MailRecipients.Add(mr1);

            var mr2 = new MailRecipient
            {
                Id = Guid.NewGuid(),
                UserId = alice.Id,
                EmailId = email2.Id,
                Type = RecipientType.To,
                ReceivedAt = email2.CreatedAt,
                IsRead = false
            };
            context.MailRecipients.Add(mr2);

            context.MailRecipientLabels.Add(new MailRecipientLabel { MailRecipientId = mr1.Id, LabelId = bobWork.Id });

            var draft = new Draft
            {
                Id = Guid.NewGuid(),
                UserId = alice.Id,
                Subject = "Meeting notes",
                Body = "Discuss Q2 roadmap and resource allocation.",
                UpdatedAt = DateTime.UtcNow
            };
            context.Drafts.Add(draft);

            await context.SaveChangesAsync();
        }
    }
}
