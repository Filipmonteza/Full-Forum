using FullForum_Domain.Entities;

namespace FullForum_Application.UseCases.Threads.CreateThread;

public sealed record CreateThreadResult(
    bool Success,
    ForumThread? Thread,
    string? Error,
    int? SuggestedStatusCode = null)
{
    /// <summary>
    /// Return success message with created thread
    /// </summary>
    public static CreateThreadResult Ok(ForumThread forumThread) =>
        new(Success: true, Thread: forumThread, Error: null);

    /// <summary>
    /// Return failed result with error message and HTTP status code
    /// </summary>
    public static CreateThreadResult Fail(string error, int? suggestedStatusCode = null) =>
        new(Success: false, Thread: null, Error: error, SuggestedStatusCode: suggestedStatusCode);
}
    
