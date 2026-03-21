namespace FullForum_Application.Interfaces;

/// <summary>
/// Service interface for retrieving user information by Id or DisplayName
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Return user with Id, otherwise null
    /// </summary>
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken cancellationToken = default);
}