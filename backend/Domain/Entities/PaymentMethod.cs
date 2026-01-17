using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public PaymentMethodType Type { get; private set; }

        protected PaymentMethod() { }

        public PaymentMethod(Guid userId, string name, PaymentMethodType type)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Type = type;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio!");

            Name = name;
            MarkAsUpdated();
        }

        public void ChangeType(PaymentMethodType type)
        {
            Type = type;
            MarkAsUpdated();
        }
    }
}
