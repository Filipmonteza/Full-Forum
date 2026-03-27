using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FullForum_Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationIdentityUser>>();

        // Roles
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = role,
                    NormalizedName = role.ToUpperInvariant()
                });

                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Users
        await EnsureUserAsync("admin@example.com", "Admin123!", "Admin", "AdminUser");
        await EnsureUserAsync("testuser1@example.com", "TestUserA123!", "User", "TestUserA1");
        await EnsureUserAsync("testuser2@example.com", "TestUserB123!", "User", "TestUserB2");

        async Task EnsureUserAsync(string email, string password, string role, string displayName)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationIdentityUser
                {
                    UserName = email,
                    Email = email,
                    DisplayName = displayName,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create user {email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                if (user.DisplayName != displayName)
                {
                    user.DisplayName = displayName;

                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to update user {email}: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to remove existing roles from user {email}: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                }
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to assign role '{role}' to user {email}: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}