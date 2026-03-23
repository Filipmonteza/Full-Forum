namespace FullForum_Application.UseCases.Comments.GetCommentsForThread;


/// <summary>
/// Command for retrieving all comments belonging to chosen thread
/// </summary>
public sealed record GetCommentForThreadCommand(
    Guid ThreadId
    );
