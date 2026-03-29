using FullForum_Application.Interfaces;
using FullForum_Application.UseCases.Comments.CreateComment;
using FullForum_Application.UseCases.Comments.GetCommentsForThread;
using FullForum_Application.UseCases.Threads.CreateThread;
using FullForum_Application.UseCases.Threads.GetThreads;
using FullForum_Infrastructure.Identity;
using FullForum_Infrastructure.Persistence;
using FullForum_Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FullForum_Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the database context, Identity, query handlers and command handlers
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ForumDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));

        // Identity
        services
            .AddIdentityCore<ApplicationIdentityUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ForumDbContext>();
        
        // Handlers
        services.AddScoped<GetThreadHandler>();
        services.AddScoped<GetCommentForThreadHandler>();
        
        // Commands
        services.AddScoped<CreateThreadHandler>();
        services.AddScoped<CreateCommentHandler>();
        
        // Repositories
        services.AddScoped<IThreadRepository, ThreadRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }
}