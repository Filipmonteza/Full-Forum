namespace FullForum_Domain.Common;

/// <summary>
/// Abstract base class for all domain entities, with audit fields and soft delete
/// </summary>
public abstract class BaseEntity : IEntity
{
    // Primary key for all entities
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Marks entity as soft deleted
    /// </summary>
    public void MarkDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets UpdatedAt timestamp to current time (Utc)
    /// </summary>
    public void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}