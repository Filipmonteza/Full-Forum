using FullForum_Domain.Entities;
using FullForum_Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullForum_Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CommentContent)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        // Comment → ApplicationUser (many-to-one)
        builder.HasOne<ApplicationIdentityUser>()
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment → Thread (many-to-one)
        builder.HasOne(x => x.Thread)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ThreadId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing comments (reply tree)
        builder.HasOne(x => x.ParentComment)
            .WithMany(x => x.ChildComments)
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}