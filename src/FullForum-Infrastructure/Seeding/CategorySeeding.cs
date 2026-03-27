using FullForum_Domain.Entities;
using FullForum_Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FullForum_Infrastructure.Seeding;

public static class CategorySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var context = serviceProvider.GetRequiredService<ForumDbContext>();

        if (await context.Categories.AnyAsync(cancellationToken))
            return;

        var defaultCategories = new[]
        {
            new Category { CategoryName = "General Gaming", CategoryDescription = "Talk about anything related to gaming." },
            new Category { CategoryName = "Game Discussions", CategoryDescription = "Discuss specific games, updates, and gameplay." },
            new Category { CategoryName = "Tips & Strategies", CategoryDescription = "Share guides, tips, and strategies to improve gameplay." },
            new Category { CategoryName = "Looking for Group (LFG)", CategoryDescription = "Find teammates and people to play with." },
            new Category { CategoryName = "Hardware & Setup", CategoryDescription = "Discuss gaming PCs, consoles, and setups." },
            new Category { CategoryName = "Esports & Competitive", CategoryDescription = "Talk about tournaments, rankings, and competitive play." },
            new Category { CategoryName = "Mods & Custom Content", CategoryDescription = "Share mods, skins, and custom content." },
            new Category { CategoryName = "Game Help", CategoryDescription = "Ask for help with missions, bugs, or mechanics." }
        };

        foreach (var category in defaultCategories)
        {
            category.Validate();
        }

        context.Categories.AddRange(defaultCategories);
        await context.SaveChangesAsync(cancellationToken);
    }
}