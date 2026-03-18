using FullForum_Domain.Common;

namespace FullForum_Domain.Entities;

/// <summary>
/// Domain entity representing a forum thread
/// </summary>
public class ForumThread : BaseEntity
{
    public string ThreadTitle { get; set; } = string.Empty;
    public string ThreadContent { get; set; } = string.Empty;
 
    // Navigation Properties
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public Guid ApplicationUserId { get; set; } 
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Validates that thread has title, content, category and user Id
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ThreadTitle))
        {
            throw new ArgumentException("Thread title cannot be empty.");
        }
        
        if (string.IsNullOrWhiteSpace(ThreadContent))
        {
            throw new ArgumentException("Thread content cannot be empty.");
        }
        
        if (CategoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.");
        }
        
        if (ApplicationUserId == Guid.Empty)
        {
            throw new ArgumentException("Application User ID cannot be empty.");
        }
    }
}