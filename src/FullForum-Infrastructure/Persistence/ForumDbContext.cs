using FullForum_Domain.Common;
using FullForum_Domain.Entities;
using FullForum_Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FullForum_Infrastructure.Persistence;

public class ForumDbContext : IdentityDbContext<ApplicationIdentityUser, IdentityRole<Guid>, Guid>
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options)
        : base(options){ }

    public DbSet<Category> Categories { get; set; } = null;
    public DbSet<Comment> Comments { get; set; } = null;
    public DbSet<ForumThread> Threads { get; set; } = null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ForumDbContext).Assembly);

        modelBuilder.Entity<ApplicationIdentityUser>(b =>
        {
            b.Property(u => u.DisplayName)
                .HasMaxLength(35)
                .IsRequired();

            b.HasIndex(u => u.DisplayName)
                .IsUnique();
        });

        modelBuilder.Entity<Category>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<Comment>()
            .HasQueryFilter(y => !y.IsDeleted);

        modelBuilder.Entity<ForumThread>()
            .HasQueryFilter(y => !y.IsDeleted);

        modelBuilder.Entity<ApplicationIdentityUser>()
            .HasQueryFilter(y => !y.IsDeleted);
    }

    public override int SaveChanges()
    {
            ApplyAuditInfo();
            return base.SaveChanges();
    }
        
    // Apply audit info before saving asynchronously
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return await base.SaveChangesAsync(cancellationToken);
    }

    // Audit Logic
    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        // Collects all tracked entities in DbContext
        var entries = ChangeTracker
            .Entries()
            .ToList();

        foreach (var entry in entries)
        {
            // Separate handling as ApplicationIdentityUser,
            // does not inherit from BaseEntity
            if (entry.Entity is ApplicationIdentityUser appUser)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        appUser.CreatedAt = now;
                        break;
                    
                    case EntityState.Modified:
                        appUser.UpdatedAt = now;
                        break;
                    
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        
                        // Mark as soft delete
                        appUser.IsDeleted = true;
                        appUser.DeletedAt = now;
                        appUser.UpdatedAt = now;
                        break;
                }
            }
            
            // Domain Entities
            else if (entry.Entity is BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = now;
                        break;
                    
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = now;
                        break;
                    
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        
                        // Mark as soft delete
                        baseEntity.IsDeleted = true;
                        baseEntity.DeletedAt = now;
                        baseEntity.UpdatedAt = now;
                        break;
                }
            }
        }
    }
}

