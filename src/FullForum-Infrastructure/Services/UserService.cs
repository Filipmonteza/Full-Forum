using FullForum_Application.Interfaces;
using FullForum_Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FullForum_Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;

    public UserService(UserManager<ApplicationIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<string?> GetDisplayNameAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);
    }
}