using FullForum_Infrastructure.Identity;

namespace FullForum_Infrastructure.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await IdentitySeeder.SeedAsync(serviceProvider, cancellationToken);
        await CategorySeeder.SeedAsync(serviceProvider, cancellationToken);
    }
}