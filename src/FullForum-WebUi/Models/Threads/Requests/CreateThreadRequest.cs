namespace FullForum_WebUi.Models.Threads;

// DTOs for Thread operations
public sealed record CreateThreadRequest(
    string ThreadTitle,
    string ThreadContent,
    Guid CategoryId
);