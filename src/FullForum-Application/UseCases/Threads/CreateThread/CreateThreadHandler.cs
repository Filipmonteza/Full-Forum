using FullForum_Application.Interfaces;
using FullForum_Domain.Entities;

namespace FullForum_Application.UseCases.Threads.CreateThread;

public sealed class CreateThreadHandler
{
    private readonly IThreadRepository _threadRepository;
    
    public CreateThreadHandler(IThreadRepository threadRepository)
        => _threadRepository = threadRepository;
    
    public async Task<CreateThreadResult> HandleAsync(CreateThreadCommand cmd, CancellationToken ct = default)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(cmd.ThreadTitle))
                return CreateThreadResult.Fail("Thread title cannot be empty.");

            if (string.IsNullOrWhiteSpace(cmd.ThreadContent))
                return CreateThreadResult.Fail("Thread content cannot be empty.");

            if (cmd.CategoryId == Guid.Empty)
                return CreateThreadResult.Fail("CategoryId cannot be empty.");

            if (cmd.ApplicationUserId == Guid.Empty)
                return CreateThreadResult.Fail("ApplicationUserId cannot be empty.");

            // Business rules
            if (!await _threadRepository.CategoryExistsAsync(cmd.CategoryId, ct))
                return CreateThreadResult.Fail($"CategoryId '{cmd.CategoryId}' does not exist.");

            if (!await _threadRepository.UserExistsAsync(cmd.ApplicationUserId, ct))
                return CreateThreadResult.Fail($"ApplicationUserId '{cmd.ApplicationUserId}' does not exist.");

            // Duplicate title check (case-insensitive)
            var duplicate = await _threadRepository.ThreadTitleExistsInCategoryAsync(cmd.CategoryId, cmd.ThreadTitle, ct);
            if (duplicate)
            {
                return CreateThreadResult.Fail(
                    $"Thread title '{cmd.ThreadTitle}' already exists in this category.",
                    suggestedStatusCode: 409);
            }

            // Create domain entity
            var thread = new ForumThread
            {
                ThreadTitle = cmd.ThreadTitle.Trim(),
                ThreadContent = cmd.ThreadContent.Trim(),
                CategoryId = cmd.CategoryId,
                ApplicationUserId = cmd.ApplicationUserId
            };

            try
            {
                thread.Validate();
            }
            catch (ArgumentException exception)
            {
                return CreateThreadResult.Fail(exception.Message);
            }
            
            // Persist via repository
            await _threadRepository.AddAsync(thread, ct);

            // Return created thread
            return CreateThreadResult.Ok(thread);
        }
}