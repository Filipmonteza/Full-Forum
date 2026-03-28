namespace FullForum_WebApi.Contracts.Threads.ThreadResponse;

/// Api response representing details of single thread
public sealed record ThreadDetailResponse(
    Guid Id,
    string ThreadTitle,
    string ThreadContent,
    Guid CategoryId,
    Guid ApplicationUserId,
    string DisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
    );