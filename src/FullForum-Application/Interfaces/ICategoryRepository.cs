using FullForum_Domain.Entities;

namespace FullForum_Application.Interfaces;

/// <summary>
/// Repository interface for category-specific queries
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>
    /// Return category matching name, otherwise null
    /// </summary>
    Task<Category> GetNameAsync(string name, CancellationToken cancellationToken = default);
}