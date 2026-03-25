using FullForum_Application.Interfaces;
using FullForum_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullForum_Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ForumDbContext _dbContext;
    
    public CategoryRepository(ForumDbContext context) => _dbContext = context;
    
    // Collect category based on ID
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    
    // Collect all categories from database
    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
    await _dbContext.Categories.AsNoTracking().ToListAsync(cancellationToken);
    
    // Add new category in database
    public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Update existing category in database
    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Mark category as deleted in database
    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Remove(entity);
        await  _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Collect category based on name 
    public async Task<Category?> GetNameAsync(string name, CancellationToken cancellationToken = default) =>
    await  _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == name, cancellationToken);
}