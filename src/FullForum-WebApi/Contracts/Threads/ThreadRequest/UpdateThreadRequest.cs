namespace FullForum_WebApi.Contracts.Threads.ThreadRequest;

/// Api request for updating title and content of thread
public sealed record UpdateThreadRequest(
    string ThreadTitle,
    string ThreadContent
);

    
