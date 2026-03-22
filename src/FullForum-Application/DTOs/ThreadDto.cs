namespace FullForum_Application.DTOs;

/// <summary>
/// DTO representing a Thread, used in thread endpoints
/// </summary>
public sealed record ThreadDto(
    Guid Id,
    string Title,
    string Content,
    Guid CategoryId,
    Guid UserId,
    DateTime CreatedAt
    );

    
