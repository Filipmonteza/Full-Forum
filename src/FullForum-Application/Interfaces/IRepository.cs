using FullForum_Domain.Common;

namespace FullForum_Application.Interfaces;

/// <summary>
/// Generic repository interface that defines standard CRUD operations for entities.
/// This abstraction follows the Repository Pattern to separate data access logic
/// from business logic.
///
/// The generic type <typeparamref name="TEntity"/> is constrained to BaseEntity,
/// ensuring all entities share common properties (e.g., Id).
///
/// All methods are asynchronous to support non-blocking I/O operations,
/// and include optional CancellationToken for better control over long-running tasks.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}