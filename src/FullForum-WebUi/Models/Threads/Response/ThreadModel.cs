using FullForum_WebUi.Models.Comments;

namespace FullForum_WebUi.Models.Threads;

public sealed class ThreadModel
{
    public Guid Id { get; set; }
    public string ThreadTitle { get; set; } = string.Empty;
    public string ThreadContent { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public string  DisplayName { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CommentModel> Comments { get; set; } = new();
}