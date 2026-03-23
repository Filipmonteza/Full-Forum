using FullForum_Domain.Entities;

namespace FullForum_Application.UseCases.Threads.GetThreads;

public sealed record GetThreadResult(
    bool Success,
    List<ForumThread>? Threads,
    string? Error,
    int? SuggestionStatusCode = null
)
{
    /// <summary>
    /// Return success result with list of threads
    /// </summary>
    public static GetThreadResult Ok(List<ForumThread> threads) =>
        new(true, threads, null);

    /// <summary>
    /// Returns fail result with error message and HTTP status code
    /// </summary>
    public static GetThreadResult Fail(string error, int? suggestedStatusCode = null) =>
        new(false, null, error, suggestedStatusCode);
}

    
