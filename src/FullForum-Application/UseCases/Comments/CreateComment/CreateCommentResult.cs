using FullForum_Domain.Entities;

namespace FullForum_Application.UseCases.Comments.CreateComment;

/// <summary>
/// Result returned by CreateCommentHandler, translated to HTTP by Api 
/// </summary>
public sealed record CreateCommentResult(bool Success, Comment? Comment, string? Error)
{
    
    /// WebApi returns success result (201 etc)
    public static CreateCommentResult Ok(Comment comment) =>
        new(true, comment, null);
    
    /// WebApi returns failed result (400, 404 etc)
    public static CreateCommentResult Fail(string error) =>
        new (false, null, error);
}
