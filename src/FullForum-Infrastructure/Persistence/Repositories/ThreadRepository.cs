using FullForum_Application.Interfaces;
using FullForum_Domain.Entities;

namespace FullForum_Infrastructure.Persistence.Repositories;

public class ThreadRepository : IThreadRepository
{
    public Task<List<ForumThread>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ThreadTitleExistsInCategoryAsync(Guid categoryId, string title, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ThreadExistsAsync(Guid threadId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ForumThread>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(ForumThread thread, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}