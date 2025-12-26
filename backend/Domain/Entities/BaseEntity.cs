namespace backend.Domain.Entities
{
  public abstract class BaseEntity
  {
    public DateTime CreatedAt { get; internal set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; internal set; }
    public bool IsDeleted { get; internal set; } = false;
    public DateTime? DeletedAt { get; internal set; }

    public void MarkAsDeleted()
    {
      IsDeleted = true;
      DeletedAt = DateTime.UtcNow;
    }

    public void MarkAsUpdated()
    {
      UpdatedAt = DateTime.UtcNow;
    }

    // Método para restaurar se necessário
    public void Restore()
    {
      IsDeleted = false;
      DeletedAt = null;
    }
  }
}

