using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FullForum_Application.UseCases.Threads.CreateThread;
using FullForum_Domain.Entities;
using FullForum_Infrastructure.Persistence;
using FullForum_WebApi.Contracts.Threads.ThreadRequest;
using FullForum_WebApi.Contracts.Threads.ThreadResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullForum_WebApi.Endpoints;

public static class ThreadEndpoints
{
    /// <summary>
    /// Create new thread for authenticated user
    /// </summary>
    public static async Task<Results<Created<ForumThread>, ProblemHttpResult>> CreateThread(
        CreateThreadRequest dto,
        ClaimsPrincipal user,
        CreateThreadHandler handler,
        CancellationToken ct)
    {
        // Get UserId from JWT
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
        
        var command = new CreateThreadCommand(
            dto.ThreadTitle,
            dto.ThreadContent,
            dto.CategoryId,
            userId
        );

        var result = await handler.HandleAsync(command, ct);

        if (!result.Success || result.Thread is null)
        {
            var statusCode = result.SuggestedStatusCode ?? StatusCodes.Status400BadRequest;

            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Failed to create thread",
                Status = statusCode,
                Detail = result.Error
            });
        }

        return TypedResults.Created($"/threads/{result.Thread.Id}", result.Thread);
    }

    /// <summary>
    /// Get threads (all, or filtered by category)
    /// </summary>
    public static async Task<Results<Ok<PagedThreadsResponse>, ProblemHttpResult>> GetThreads(
        ForumDbContext context,
        CancellationToken ct,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = "date",
        [FromQuery] bool descending = true)
    {
        var q = context.Threads
            .Where(t => !t.IsDeleted);

        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            q = q.Where(t => t.CategoryId == categoryId.Value);
        }

        var totalCount = await q.CountAsync(ct);

        // Sort on entity fields before mapping to DTO, join with Users to include DisplayName
        var rawQuery = from t in q.AsNoTracking()
                       join u in context.Users.AsNoTracking()
                       on t.ApplicationUserId equals u.Id into users
                       from u in users.DefaultIfEmpty()
                       select new { t, u };

        // Sort on raw entities so EF Core translates to SQL
        var ordered = (sortBy?.ToLower(), descending) switch
        {
            ("comments", true) => rawQuery.OrderByDescending(x => x.t.Comments.Count(c => !c.IsDeleted)),
            ("comments", false) => rawQuery.OrderBy(x => x.t.Comments.Count(c => !c.IsDeleted)),
            (_, true) => rawQuery.OrderByDescending(x => x.t.UpdatedAt ?? x.t.CreatedAt),
            (_, false) => rawQuery.OrderBy(x => x.t.UpdatedAt ?? x.t.CreatedAt)
        };

        // Skip threads from previous pages, project to DTO after sorting and pagination
        var threads = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ThreadListItemResponse(
                x.t.Id,
                x.t.ThreadTitle,
                x.t.ThreadContent,
                x.t.CategoryId,
                x.t.ApplicationUserId,
                x.u != null ? x.u.DisplayName : "Unknown",
                x.t.CreatedAt,
                x.t.UpdatedAt,
                x.t.Comments.Count(c => !c.IsDeleted)
             ))
            .ToListAsync(ct);
        return TypedResults.Ok(new PagedThreadsResponse(threads, totalCount));
    }

    /// <summary>
    /// Get a single thread, otherwise problem result
    /// </summary>
    public static async Task<Results<Ok<ThreadDetailResponse>, ProblemHttpResult>> GetThread(
        [FromRoute] Guid id,
        ForumDbContext context,
        CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Empty id",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Thread id cannot be empty."
            });
        }

        var thread = await (
            from t in context.Threads.AsNoTracking()
            where t.Id == id && !t.IsDeleted
            join u in context.Users.AsNoTracking()
                on t.ApplicationUserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            select new ThreadDetailResponse(
                t.Id,
                t.ThreadTitle,
                t.ThreadContent,
                t.CategoryId,
                t.ApplicationUserId,
                u != null ? u.DisplayName : "Unknown",
                t.CreatedAt,
                t.UpdatedAt
            )
        ).FirstOrDefaultAsync(ct);

        if (thread is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Thread not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Thread with id {id} could not be found."
            });
        }

        return TypedResults.Ok(thread);
    }

    /// <summary>
    /// Update the title and content of a thread
    /// </summary>
    public static async Task<Results<NoContent, ProblemHttpResult>> UpdateThread(
        [FromRoute] Guid id,
        UpdateThreadRequest dto,
        ForumDbContext context,
        CancellationToken ct)
    {
        var thread = await context.Threads
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

        if (thread is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Thread not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        if (string.IsNullOrWhiteSpace(dto.ThreadTitle) ||
            string.IsNullOrWhiteSpace(dto.ThreadContent))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Missing title/content",
                Status = StatusCodes.Status400BadRequest,
                Detail = "ThreadTitle and ThreadContent are required."
            });
        }

        thread.ThreadTitle = dto.ThreadTitle.Trim();
        thread.ThreadContent = dto.ThreadContent.Trim();
        thread.MarkUpdated();

        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Soft delete a thread and all its comments. Only for owner or admin
    /// </summary>
    public static async Task<Results<NoContent, ProblemHttpResult>> DeleteThread(
        [FromRoute] Guid id,
        ClaimsPrincipal user,
        ForumDbContext context,
        CancellationToken ct)
    {
        var userIdStr =
            user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            user.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Validate user from token
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid token",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Could not read user id from token."
            });
        }

        // Get thread to verify ownership
        var thread = await context.Threads
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

        if (thread is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Thread not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Thread with id {id} could not be found."
            });
        }
        
        // Only owner or admin can delete thread
        var isAdmin = user.IsInRole("Admin");
        var isOwner = thread.ApplicationUserId == userId;
        
        if (!isAdmin && !isOwner)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "You do not have permission to delete this thread."
            });
        }
        
        // Soft delete thread
        thread.MarkDeleted();
        
        // Soft delete all comments in thread
        var comments = await context.Comments
            .Where(c => c.ThreadId == id && !c.IsDeleted)
            .ToListAsync(ct);
        
        foreach (var comment in comments)
        {
            comment.MarkDeleted();
        }

        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}