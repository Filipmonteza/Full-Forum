namespace FullForum_Application.UseCases.Threads.GetThreads;

/// <summary>
/// Command for retrieving all threads from category
/// </summary>
public sealed record GetThreadCommand(Guid CategoryId);

    
