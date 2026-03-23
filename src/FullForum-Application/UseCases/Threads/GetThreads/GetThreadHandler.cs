using FullForum_Application.Interfaces;

namespace FullForum_Application.UseCases.Threads.GetThreads;

public sealed class GetThreadHandler
{
    private readonly IThreadRepository _threadRepository;
    
    public GetThreadHandler(IThreadRepository threadRepository) 
        => _threadRepository = threadRepository;
    
    /// <summary>
    /// Validates Command and fetches all threads for category from repository
    /// </summary>
    public async Task<GetThreadResult> HandleAsync(
        GetThreadCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.CategoryId == Guid.Empty)
            return GetThreadResult.Fail("Invalid CategoryId");

        if (!await _threadRepository.CategoryExistsAsync(command.CategoryId, cancellationToken))
            return GetThreadResult.Fail($"Category with id '{command.CategoryId}' does not exist.");

        var threads = await _threadRepository.GetByCategoryIdAsync(command.CategoryId, cancellationToken);
        return GetThreadResult.Ok(threads);
    }
}