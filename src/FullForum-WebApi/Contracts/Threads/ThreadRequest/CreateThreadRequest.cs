namespace FullForum_WebApi.Contracts.Threads.ThreadRequest;

/// Api request for creating new thread in category
public sealed record CreateThreadRequest(
    string ThreadTitle,
    string ThreadContent,
    Guid CategoryId
);

    
