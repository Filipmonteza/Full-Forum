using FullForum_Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullForum_Infrastructure.Persistence.Configurations;

// EF Core Configuration for ApplicationIdentityUser, mapping audit fields to database
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationIdentityUser>
{
    /// <summary>
    /// Configure column mapping for audit properties
    /// </summary>
    public void Configure(EntityTypeBuilder<ApplicationIdentityUser> builder)
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.IsDeleted);
        builder.Property(x => x.DeletedAt);
    }
}