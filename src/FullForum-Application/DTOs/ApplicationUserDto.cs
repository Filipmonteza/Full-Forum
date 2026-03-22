namespace FullForum_Application.DTOs;

/// <summary>
/// DTO representing a user, used in admin and user-facing endpoints
/// </summary>
public sealed record ApplicationUserDto(
    Guid Id,
    string Username,
    string Email,
    DateTime CreatedAt
    );
