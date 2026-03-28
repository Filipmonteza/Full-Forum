using FullForum_Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FullForum_WebApi.EndPoints;

public static class AdminEndpoints
{
    /// <summary>
    /// Registers admin endpoints under /api/admin route group, require admin policy
    /// </summary>
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/admin").RequireAuthorization("Admin");

        group.MapGet("/users", async (UserManager<ApplicationIdentityUser> userManager) =>
        {
            // Return only relevant fields instead of full user object
            var users = await userManager.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.CreatedAt,
                u.UpdatedAt

            }).ToListAsync();

            return Results.Ok(users);
        });

        // Assign admin role to user
        group.MapPost("/assign-admin", async (string email, UserManager<ApplicationIdentityUser> userManager) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Results.NotFound($"User with email '{email}' not found.");

            var result = await userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
                return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Results.Ok($"User '{email}' has been assigned the Admin role.");
        });
        return endpoints;
    }
}