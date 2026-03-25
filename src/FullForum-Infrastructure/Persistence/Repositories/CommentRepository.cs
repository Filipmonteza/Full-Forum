using FullForum_Application.Interfaces;
using FullForum_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullForum_Infrastructure.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ForumDbContext _dbContext;
    public CommentRepository(ForumDbContext dbContext) => _dbContext = dbContext;

    // Collects comment based on Id, otherwise return null
    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
    await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    

    // Collects all comments without tracking them in DbContext
    public async Task<List<Comment>> GetAllAsync(CancellationToken cancellationToken = default) =>
    await _dbContext.Comments.AsNoTracking().ToListAsync(cancellationToken);
    
    
    // Adds new comment and saves it
    public async Task AddAsync(Comment entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Comments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Update comment
    public async Task UpdateAsync(Comment entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Comments.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Soft delete comment
    public async Task DeleteAsync(Comment entity, CancellationToken cancellationToken = default)
    {
        entity.MarkDeleted();
        _dbContext.Comments.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Collect all comments from specific thread, sorted by creation
    public async Task<List<Comment>> GetCommentsByThreadIdAsync(Guid threadId, CancellationToken cancellationToken = default) =>
        await _dbContext.Comments
            .IgnoreQueryFilters()
            .Where(x => x.ThreadId == threadId)
            .OrderBy(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    
    // Check if thread exists in database
    public async Task<bool> ThreadExistsAsync(Guid threadId, CancellationToken cancellationToken = default) =>
        await _dbContext.Threads.AnyAsync(t => t.Id == threadId, cancellationToken);
    
    // check if user with ID exists in database
    public Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default) =>
    _dbContext.Users.AnyAsync(t => t.Id == userId, cancellationToken);
    
    // Collect Comment based on Id, otherwise return null
    public Task<Comment?> GetParentCommentAsync(Guid parentCommentId, CancellationToken cancellationToken = default) =>
        _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == parentCommentId, cancellationToken);
    
    // Collect all comments from specific user, sorted by creation
    public async Task<List<Comment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.Comments
            .Include(c => c.Thread)
            .Where(c => c.ApplicationUserId == userId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}