namespace FullForum_WebUi.Models.Comments.Requests;

// Creating Comment
public sealed record CommentCreateDto(
    string CommentContent,
    Guid ThreadId,
    Guid? ParentCommentId = null
);

    
