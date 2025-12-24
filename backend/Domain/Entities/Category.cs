namespace backend.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public Guid? ParentCategoryId { get; private set; }

        protected Category() { } // EF

        public Category(Guid userId, string name, Guid? parentCategoryId = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            ParentCategoryId = parentCategoryId;
        }

        public void Rename(string name)
        {
            Name = name;
        }

    }
}