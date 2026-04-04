namespace FullForum_WebUi.Models.Threads;

// DTO for updating a thread (title and content)
public record UpdateThreadRequest(
    string ThreadTitle, 
    string ThreadContent
    );