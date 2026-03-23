using FullForum_Domain.Entities;

namespace FullForum_Application.UseCases.Comments.GetCommentsForThread;


/// <summary>
/// Result returned by GetCommentsForThreadHandler, translated to HTTP responses på Api
/// </summary>
public sealed record GetCommentForThreadResult(bool Success, List<Comment>? Comments, string? Error)
{
    
    /// <summary>
    /// Return success result with list of comments
    /// </summary>
    public static GetCommentForThreadResult Ok(List<Comment>? comments) =>
        new(true, comments, null);

    /// <summary>
    /// Return fail result
    /// </summary>
    public static GetCommentForThreadResult Fail(string error) =>
        new(false, null, error);
}

    
