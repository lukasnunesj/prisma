namespace backend.Domain.Entities
{
    public class Category : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Color { get; private set; } = null!;
        public string? Icon { get; private set; }
        public Guid? ParentCategoryId { get; private set; }

        protected Category() { } // EF

        public Category(Guid userId, string name, string color, string? icon = null, Guid? parentCategoryId = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Color = color;
            Icon = icon;
            ParentCategoryId = parentCategoryId;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio!");
            Name = name;
            MarkAsUpdated();
        }

        public void ChangeColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Nome não pode ser vazio!");

            Color = color;
            MarkAsUpdated();
        }

        public void ChangeIcon(string? icon)
        {
            Icon = icon;
        }

    }
}