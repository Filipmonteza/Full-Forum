using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FullForum_Application.UseCases.Comments.CreateComment;
using FullForum_Application.UseCases.Comments.GetCommentsForThread;
using FullForum_Infrastructure.Identity;
using FullForum_Infrastructure.Persistence;
using FullForum_WebApi.Contracts.Comments.CommentRequest;
using FullForum_WebApi.Contracts.Comments.CommentResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullForum_WebApi.Endpoints;

/// <summary>
/// Minimal Api endpoints for comment operations
/// </summary>
public static class CommentEndpoints
{
    /// <summary>
    /// Create a new comment or reply
    /// </summary>
    public static async Task<Results<Created<CommentResponse>, ProblemHttpResult>> CreateComment(
        CreateCommentRequest dto,
        ClaimsPrincipal user,
        CreateCommentHandler handler,
        UserManager<ApplicationIdentityUser> userManager,
        CancellationToken ct)
    {
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

        var command = new CreateCommentCommand(
            dto.CommentContent,
            dto.ThreadId,
            userId,
            dto.ParentCommentId
        );

        var result = await handler.HandleAsync(command, ct);

        if (!result.Success || result.Comment is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Failed to create comment",
                Status = StatusCodes.Status400BadRequest,
                Detail = result.Error ?? "Comment could not be created."
            });
        }

        var c = result.Comment;

        var appUser = await userManager.FindByIdAsync(c.ApplicationUserId.ToString());

        var displayName = !string.IsNullOrWhiteSpace(appUser?.DisplayName)
            ? appUser.DisplayName
            : !string.IsNullOrWhiteSpace(appUser?.UserName)
                ? appUser.UserName
                : "Unknown User";

        return TypedResults.Created(
            $"/comments/{c.Id}",
            new CommentResponse(
                c.Id,
                c.ThreadId,
                c.ApplicationUserId,
                c.ParentCommentId,
                c.CommentContent,
                displayName,
                c.CreatedAt,
                c.IsDeleted
            )
        );
    }

    /// <summary>
    /// Get all comments from a thread, with display names from Identity
    /// </summary>
    public static async Task<Results<Ok<List<CommentResponse>>, ProblemHttpResult>> GetThreadComments(
        [FromRoute] Guid threadId,
        GetCommentForThreadHandler handler,
        UserManager<ApplicationIdentityUser> userManager,
        CancellationToken ct)
    {
        if (threadId == Guid.Empty)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid ThreadId",
                Status = StatusCodes.Status400BadRequest,
                Detail = "ThreadId cannot be empty."
            });
        }

        var query = new GetCommentForThreadCommand(threadId);
        var result = await handler.HandleAsync(query, ct);

        if (!result.Success || result.Comments is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Failed to get comments",
                Status = StatusCodes.Status404NotFound,
                Detail = result.Error ?? "Comments not found."
            });
        }

        var responses = new List<CommentResponse>();

        foreach (var c in result.Comments)
        {
            var appUser = await userManager.FindByIdAsync(c.ApplicationUserId.ToString());

            var displayName = !string.IsNullOrWhiteSpace(appUser?.DisplayName)
                ? appUser.DisplayName
                : !string.IsNullOrWhiteSpace(appUser?.UserName)
                    ? appUser.UserName
                    : "Unknown User";

            responses.Add(new CommentResponse(
                c.Id,
                c.ThreadId,
                c.ApplicationUserId,
                c.ParentCommentId,
                c.CommentContent,
                displayName,
                c.CreatedAt,
                c.IsDeleted
            ));
        }

        return TypedResults.Ok(responses);
    }

    /// <summary>
    /// Update content of a comment
    /// </summary>
    public static async Task<Results<NoContent, ProblemHttpResult>> UpdateComment(
        [FromRoute] Guid id,
        UpdateCommentRequest dto,
        ForumDbContext context,
        CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Empty id",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Id cannot be empty."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.CommentContent))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid comment content",
                Status = StatusCodes.Status400BadRequest,
                Detail = "CommentContent cannot be empty."
            });
        }

        var comment = await context.Comments.FirstOrDefaultAsync(
            c => c.Id == id && !c.IsDeleted,
            ct);

        if (comment is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Comment not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        comment.CommentContent = dto.CommentContent.Trim();
        comment.MarkUpdated();

        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Soft delete a comment. Only for owner or admin
    /// </summary>
    public static async Task<Results<NoContent, ProblemHttpResult>> DeleteComment(
        [FromRoute] Guid id,
        ClaimsPrincipal user,
        ForumDbContext context,
        CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Empty id",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Id cannot be empty."
            });
        }

        var comment = await context.Comments.FirstOrDefaultAsync(
            c => c.Id == id && !c.IsDeleted,
            ct);

        if (comment is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Comment not found",
                Status = StatusCodes.Status404NotFound
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

        var isAdmin = user.IsInRole("Admin");

        if (!isAdmin && comment.ApplicationUserId != userId)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "You do not have permission to delete this comment."
            });
        }

        comment.CommentContent = string.Empty;
        comment.MarkDeleted();

        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}