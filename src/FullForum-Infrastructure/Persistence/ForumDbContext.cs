using FullForum_Domain.Entities;
using FullForum_Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

    private void ApplyAuditInfo()
    {
            
    }
}

