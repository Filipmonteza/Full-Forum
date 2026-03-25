using FullForum_Application.Interfaces;
using FullForum_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullForum_Infrastructure.Persistence.Repositories;

public class ThreadRepository : IThreadRepository
{
     private readonly ForumDbContext _dbContext;
    public ThreadRepository(ForumDbContext dbContext) => _dbContext = dbContext;
    
    // Collect thread based on ID, otherwise return null
    public async Task<ForumThread?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Threads.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    
    // Collect all threads without tracking in DbContext
    public async Task<List<ForumThread>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Threads.AsNoTracking().ToListAsync(cancellationToken);
    
    // Add thread
    public async Task AddAsync(ForumThread entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Threads.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Update thread
    public async Task UpdateAsync(ForumThread entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Threads.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Soft delete thread
    public Task DeleteAsync(ForumThread entity, CancellationToken cancellationToken = default)
    { 
        entity.MarkDeleted();
        _dbContext.Threads.Update(entity);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Collects all threads from specific category, sorted by creation
    public  async Task<List<ForumThread>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        await _dbContext.Threads
            .Where(x => x.CategoryId == categoryId)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    
    // Check if category with ID exists in database
    public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        await _dbContext.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);

    // Collect all threads from specific user, sorted by creation
    public async Task<List<ForumThread>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Threads
            .Where(t => t.ApplicationUserId == userId && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // Check if user with ID exists in database
    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
     
    // Check if duplicate thread already exists in selected category (case-insensitive)
    public async Task<bool> ThreadTitleExistsInCategoryAsync(Guid categoryId, string title, CancellationToken cancellationToken = default) =>
        await _dbContext.Threads
            .AnyAsync(t => t.CategoryId == categoryId && t.ThreadTitle.ToLower() == title.ToLower(), cancellationToken);
}