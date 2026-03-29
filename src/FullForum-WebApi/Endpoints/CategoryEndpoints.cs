using FullForum_Infrastructure.Persistence;
using FullForum_WebApi.Contracts.Categories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullForum_WebApi.Endpoints;

/// <summary>
/// Minimal Api endpoints for category operations
/// </summary>
public static class CategoryEndpoints
{
    /// <summary>
    /// Returns all categories ordered by creation
    /// </summary>
    public static async Task<Ok<List<CategoryResponse>>> GetCategories(ForumDbContext context)
    {
        // Select converts entities to DTO
        var categories = await context.Categories
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CategoryResponse(
                c.Id,
                c.CategoryName,
                c.CategoryDescription))
            .ToListAsync();

        return TypedResults.Ok(categories);
    }

    /// <summary>
    /// Return single category by ID, otherwise problem result
    /// </summary>
    public static async Task<Results<Ok<CategoryResponse>, ProblemHttpResult>> GetCategory(
        [FromRoute] Guid id,
        ForumDbContext context)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Empty Id",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Id cannot be empty."
            });
        }

        var category = await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponse(
                c.Id,
                c.CategoryName,
                c.CategoryDescription))
            .FirstOrDefaultAsync();

        if (category is null)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Category Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Category with ID '{id}' was not found."
            });
        }

        return TypedResults.Ok(category);
    }
}