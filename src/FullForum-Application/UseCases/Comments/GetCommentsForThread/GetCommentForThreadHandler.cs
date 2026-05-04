using FullForum_Application.Interfaces;

namespace FullForum_Application.UseCases.Comments.GetCommentsForThread;

/// <summary>
/// Handles GetCommentsForThreadQuery and return all comments for thread
/// </summary>
public sealed record GetCommentForThreadHandler
{
    private readonly ICommentRepository _repository;
    
    public GetCommentForThreadHandler(ICommentRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Fetch all comments for thread from repository
    /// </summary>
    public async Task<GetCommentForThreadResult> HandleAsync(
        GetCommentForThreadCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.ThreadId == Guid.Empty)
            return GetCommentForThreadResult.Fail("Thread cannot be Empty");

        var comments = await _repository.GetCommentsByThreadIdAsync(command.ThreadId, cancellationToken);
        return GetCommentForThreadResult.Ok(comments);
    }

}