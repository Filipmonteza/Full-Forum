using FullForum_Domain.Entities;

namespace FullForum_Application.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Return all comments for thread, sorted by creation
    /// </summary>
    Task<List<Comment>> GetCommentsByThreadIdAsync(Guid threadId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Return true if thread with Id exists
    /// </summary>
    Task<bool> ThreadExistsAsync(Guid threadId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Return true if user with Id exists
    /// </summary>
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Return parent comment for Id, otherwise null
    /// </summary>
    Task<Comment?> GetParentCommentAsync(Guid threadId, Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns all comments by specific user, sorted by creation date
    /// </summary>
    Task<List<Comment>> GetByUserIdAsync(Guid parentCommentId, CancellationToken cancellationToken = default);
    
}  