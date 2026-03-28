namespace FullForum_WebApi.Contracts.Threads.ThreadResponse;

// Api response representing thread in list, including comment count
public sealed record ThreadListItemResponse(
    Guid Id,
    string ThreadTitle,
    string ThreadContent,
    Guid CategoryId,
    Guid ApplicationUserId,
    string DisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int CommentCount
    );

    
