namespace FullForum_WebUi.Models.Comments;

// ViewModel for displaying comment details in the UI, including user information and timestamps
public sealed class CommentModel
{
    public Guid Id { get; set; }
    public string CommentContent { get; set; } = string.Empty;

    public Guid ThreadId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public string DisplayName { get; set; } = string.Empty; 
    public Guid? ParentCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}