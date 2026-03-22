using FullForum_Domain.Entities;

namespace FullForum_Application.Interfaces;

/// <summary>
/// Repository interface for thread-specific queries
/// </summary>
public interface IThreadRepository
{
    /// <summary>
    /// Return all threads in given category
    /// </summary>
    Task<List<ForumThread>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if category with Id exists
    /// </summary>
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if thread with duplicate title exists
    /// </summary>
    Task<bool> ThreadTitleExistsInCategoryAsync(Guid categoryId, string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if user with Id exists
    /// </summary>
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return all threads by user
    /// </summary>
    Task<List<ForumThread>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
