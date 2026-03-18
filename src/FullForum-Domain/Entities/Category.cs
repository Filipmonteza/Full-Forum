using FullForum_Domain.Common;

namespace FullForum_Domain.Entities;


/// <summary>
/// Domain entity representing forum category
/// </summary>
public class Category : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    
    // Navigation Properties
    public ICollection<ForumThread> Threads { get; set; } = new List<ForumThread>();

    /// <summary>
    /// Validates that category has name and description
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            throw new ArgumentException("Category name cannot be empty.");
        }
        
        if (string.IsNullOrWhiteSpace(CategoryDescription))
        {
            throw new ArgumentException("Category description cannot be empty.");
        }
    }
}