namespace FullForum_Domain.Common;


/// <summary>
/// Defines identity and audit properties shared by all domain entities
/// </summary>
public interface IEntity
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}