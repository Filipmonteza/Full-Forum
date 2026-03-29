using System.Security.Claims;
using FullForum_Infrastructure.Persistence;
using FullForum_WebApi.EndPoints;

namespace FullForum_WebApi.Endpoints;

public static class MapEndpointsExtensions
{
    /// <summary>
    /// Registers all endpoint groups for threads, categories, comments, users and auth
    /// </summary>
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        // Divide mapping by resource for clarity
        app.MapAuthEndpoints();
        MapThreads(app);
        MapCategories(app);
        MapComments(app);
        MapUsers(app);
        app.MapAdminEndpoints();
        return app;
    }

    /// <summary>
    /// Registers all thread-related endpoints
    /// </summary>
    private static void MapThreads(IEndpointRouteBuilder app)
    {
        // RequireAuthorization connects policy names to roles defined in Program.cs
        // POST /threads
        // CreateThread
        app.MapPost("/threads", ThreadEndpoints.CreateThread)
            .RequireAuthorization()
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithName(nameof(ThreadEndpoints.CreateThread))
            .WithDescription("Creates a thread");

        // GET /threads
        // Return all active threads (not soft deleted)
        app.MapGet("/threads", ThreadEndpoints.GetThreads)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .WithName(nameof(ThreadEndpoints.GetThreads))
            .WithDescription("Gets all active threads");

        // GET /thread by ID
        // Return specific thread
        app.MapGet("/threads/{id:guid}", ThreadEndpoints.GetThread)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(ThreadEndpoints.GetThread))
            .WithDescription("Gets a thread by ID");

        // PUT /threads
        // Update title and content of thread
        app.MapPut("/threads/{id:guid}", ThreadEndpoints.UpdateThread)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(ThreadEndpoints.UpdateThread))
            .WithDescription("Updates a thread");

        // DELETE /threads
        // Soft delete thread
        app.MapDelete("/threads/{id:guid}",
                (Guid id, ClaimsPrincipal user, ForumDbContext context, CancellationToken ct) =>
                    ThreadEndpoints.DeleteThread(id, user, context, ct))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .WithName(nameof(ThreadEndpoints.DeleteThread))
            .WithDescription("Soft deletes a thread.");
    }

    /// <summary>
    /// Register all category-related endpoints
    /// </summary>
    private static void MapCategories(IEndpointRouteBuilder app)
    {
        // GET /categories
        // Return all categories
        app.MapGet("/categories", CategoryEndpoints.GetCategories)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .WithName(nameof(CategoryEndpoints.GetCategories))
            .WithDescription("Gets all categories.");

        // GET /categories/{id}
        // Return specific category
        app.MapGet("/categories/{id:guid}", CategoryEndpoints.GetCategory)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(CategoryEndpoints.GetCategory))
            .WithDescription("Gets a category by id.");
    }

    /// <summary>
    /// Registers all comment-relatede endpoints
    /// </summary>
    private static void MapComments(IEndpointRouteBuilder app)
    {
        // POST /comments
        // Create comment
        app.MapPost("/comments", CommentEndpoints.CreateComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(CommentEndpoints.CreateComment))
            .WithDescription("Creates a comment.");

        // GET /threads/comments
        // Return all comments in specific thread
        app.MapGet("/threads/{threadId:guid}/comments", CommentEndpoints.GetThreadComments)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(CommentEndpoints.GetThreadComments))
            .WithDescription("Gets all comments for a thread.");

        // PUT /comments
        // Update comment
        app.MapPut("/comments/{id:guid}", CommentEndpoints.UpdateComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(CommentEndpoints.UpdateComment))
            .WithDescription("Updates a comment.");

        // DELETE /comments
        // Soft delete comment
        app.MapDelete("/comments/{id:guid}", CommentEndpoints.DeleteComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(CommentEndpoints.DeleteComment))
            .WithDescription("Soft deletes a comment.");
    }

    /// <summary>
    /// Registers all user-related endpoints
    /// </summary>
    private static void MapUsers(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{userId:guid}/activity", UserEndpoints.GetUserActivity)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(UserEndpoints.GetUserActivity))
            .WithDescription("Gets a user's activity including threads and comments.");
        
        app.MapPut("/users/profile", UserEndpoints.UpdateProfile)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(UserEndpoints.UpdateProfile))
            .WithDescription("Updates current user's profile.");

        app.MapPut("/users/password", UserEndpoints.ChangePassword)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(UserEndpoints.ChangePassword))
            .WithDescription("Changes current user's password.");
    }
}