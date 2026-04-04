namespace FullForum_WebUi.Models.Category;

/// <summary>
/// Data Transfer Object (DTO) for representing a forum category.
/// This DTO is used to transfer category data between the server and client.
/// </summary>
public sealed record CategoryDto(
    Guid Id,
    string CategoryName,
    string CategoryDescription
);