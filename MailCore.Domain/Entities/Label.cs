namespace MailCore.Domain.Entities
{
    public class Label
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public string Name { get; private set; } = null!;
        public string Color { get; private set; } = null!;
        public bool IsSystemLabel { get; private set; }

        public ICollection<MailRecipientLabel> MailRecipients { get; private set; } = new List<MailRecipientLabel>();

        public static Label Create(Guid userId, string name, string color, bool isSystem = false, Guid? id = null)
        {
            return new Label
            {
                Id = id ?? Guid.NewGuid(),
                UserId = userId,
                Name = name,
                Color = color,
                IsSystemLabel = isSystem
            };
        }

        public void Update(string name, string color)
        {
            Name = name;
            Color = color;
        }

        public void MarkAsSystem() => IsSystemLabel = true;
    }
}
