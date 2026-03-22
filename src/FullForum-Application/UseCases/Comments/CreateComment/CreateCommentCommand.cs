namespace FullForum_Application.UseCases.Comments.CreateComment;

/// <summary>
/// Command for creating new comment or reply on thread
/// ParentCommentId is null for top-level comments, set for replies
/// </summary>
public sealed record CreateCommentCommand(
    string CommentContent,
    Guid ThreadId,
    Guid ApplicationUserId,
    Guid? ParentCommentId = null
    );