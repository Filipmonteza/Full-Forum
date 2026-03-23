namespace FullForum_Application.UseCases.Threads.CreateThread;

/// <summary>
/// Command for create new thread in category
/// </summary>
/// <param name="ThreadTitle"></param>
/// <param name="ThreadContent"></param>
/// <param name="CategoryId"></param>
/// <param name="ApplicationUserId"></param>
public sealed record CreateThreadCommand(
    string ThreadTitle,
    string ThreadContent,
    Guid CategoryId,
    Guid ApplicationUserId
);

    
