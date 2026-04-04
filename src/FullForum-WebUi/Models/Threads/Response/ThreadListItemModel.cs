namespace FullForum_WebUi.Models.Threads;

// DTO for a thread item in the thread list, used to display thread summaries
public sealed class ThreadListItemModel
{ 
    public Guid Id { get; set; }
    public string ThreadTitle { get; set; } = string.Empty;
    public string ThreadContent { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public string DisplayName { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CommentCount { get; set; }
}