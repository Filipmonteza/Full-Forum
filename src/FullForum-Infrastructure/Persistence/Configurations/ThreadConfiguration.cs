using FullForum_Domain.Entities;
using FullForum_Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullForum_Infrastructure.Persistence.Configurations;

public class ThreadConfiguration : IEntityTypeConfiguration<ForumThread>
{
    public void Configure(EntityTypeBuilder<ForumThread> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ThreadTitle)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.ThreadContent)
            .IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        
        // Thread → Category (many-to-one)
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Threads)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Thread → ApplicationUser (many-to-one)
        builder.HasOne<ApplicationIdentityUser>()
            .WithMany(x => x.Threads)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Thread → Comments (one-to-many)
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Thread)
            .HasForeignKey(x => x.ThreadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}