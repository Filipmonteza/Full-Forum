using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FullForum_Application.UseCases.Users;
using FullForum_Infrastructure.Identity;
using FullForum_Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullForum_WebApi.Endpoints;

/// <summary>
/// Implement endpoints related to users, eg fetching user activity, update profile
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// Return user activity, including threads and comments they created
    /// </summary>
    public static async Task<IResult> GetUserActivity(
        Guid userId,
        GetUserActivityHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new GetUserActivityCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);
        return Results.Ok(result);
    }

    /// <summary>
    /// Request Dto for updating user profile (name or email)
    /// </summary>
    public sealed record UpdateUserProfileRequest(
        string? DisplayName = null,
        string? Email = null
    );

    /// <summary>
    /// Request Dto for updating user password
    /// </summary>
    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );

    /// <summary>
    /// Update display name and/or email of user
    /// </summary>
    public static async Task<Results<Ok, ProblemHttpResult>> UpdateProfile(
        UpdateUserProfileRequest dto,
        ClaimsPrincipal user,
        UserManager<ApplicationIdentityUser> userManager,
        ForumDbContext context,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.DisplayName) && string.IsNullOrWhiteSpace(dto.Email))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one field must be provided."
            });
        }

        var userIdStr =
            user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid token",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Could not read user id from token."
            });
        }

        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "User not found",
                Status = StatusCodes.Status404NotFound,
                Detail = "The current user could not be found."
            });
        }

        if (!string.IsNullOrWhiteSpace(dto.DisplayName))
        {
            var displayName = dto.DisplayName.Trim();

            if (displayName.Length < 3 || displayName.Length > 50)
            {
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "Invalid DisplayName",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "DisplayName must be between 3 and 50 characters."
                });
            }

            var displayTaken = await context.Users
                .AnyAsync(u => u.DisplayName == displayName && u.Id != userId, ct);

            if (displayTaken)
            {
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "DisplayName taken",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "DisplayName is already taken."
                });
            }

            appUser.DisplayName = displayName;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var email = dto.Email.Trim();

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(email))
            {
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "Invalid email",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Email format is invalid."
                });
            }

            var emailTaken = await userManager.FindByEmailAsync(email);
            if (emailTaken is not null && emailTaken.Id != userId)
            {
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "Email taken",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Email is already taken."
                });
            }

            appUser.Email = email;
            appUser.UserName = email;
            appUser.NormalizedEmail = email.ToUpperInvariant();
            appUser.NormalizedUserName = email.ToUpperInvariant();
        }

        var result = await userManager.UpdateAsync(appUser);
        if (!result.Succeeded)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Update failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        return TypedResults.Ok();
    }

    /// <summary>
    /// Change password of user
    /// </summary>
    public static async Task<Results<Ok, ProblemHttpResult>> ChangePassword(
        ChangePasswordRequest dto,
        ClaimsPrincipal user,
        UserManager<ApplicationIdentityUser> userManager)
    {
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
            string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "CurrentPassword and NewPassword are required."
            });
        }

        var userIdStr =
            user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid token",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Could not read user id from token."
            });
        }

        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "User not found",
                Status = StatusCodes.Status404NotFound,
                Detail = "The current user could not be found."
            });
        }

        var result = await userManager.ChangePasswordAsync(
            appUser,
            dto.CurrentPassword,
            dto.NewPassword);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Password change failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        return TypedResults.Ok();
    }
}
