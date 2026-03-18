using FullForum_Domain.Common;

namespace FullForum_Domain.Entities;


/// <summary>
/// Domain entity representing comment or reply on forum thread
/// </summary>
public class Comment : BaseEntity
{
    public string CommentContent { get; set; } = string.Empty;
    
    // Navigation Properties
    public Guid ThreadId { get; set; }
    public ForumThread Threads { get; set; } = null!;
    
    public Guid ApplicationUserId { get; set; }
    
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    
    // Child comments replies to this comment
    public ICollection<Comment> ChildComments { get; set; } = new List<Comment>();

    /// <summary>
    /// Validates that the comment has content, thread and user Id
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(CommentContent))
        {
            throw new ArgumentException("Comment content cannot be empty.");
        }
        
        if (ThreadId == Guid.Empty)
        {
            throw new ArgumentException("ThreadId must be a valid GUID.");
        }
        
        if (ApplicationUserId == Guid.Empty)
        {
            throw new ArgumentException("ApplicationUserId must be a valid GUID.");
        }
    }
}