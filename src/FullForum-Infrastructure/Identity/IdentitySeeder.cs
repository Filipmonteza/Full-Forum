using FullForum_Domain.Entities;
using FullForum_Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
         var context = serviceProvider.GetRequiredService<ForumDbContext>();
            
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

         // Default categories
         if (!await context.Categories.AnyAsync(cancellationToken))
         {
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
                
             // Validate domain-invariants before save
             foreach (var category in defaultCategories)
             {
                 category.Validate();
             }
                
             context.Categories.AddRange(defaultCategories);
             await context.SaveChangesAsync(cancellationToken);
         }

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
                 // Update changed display name
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

             // Reassign roles
             var currentRoles = await userManager.GetRolesAsync(user);
             if (currentRoles.Any())
             {
                 var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                 if (!removeResult.Succeeded)
                 {
                        throw new Exception($"Failed to remove existing roles from user {email}: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
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