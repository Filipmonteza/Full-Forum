using FullForum_Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FullForum_Infrastructure.Identity;

public class ApplicationIdentityUser : IdentityUser<Guid>
{
    // Additional properties
    public string DisplayName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    
    // Navigation Properties
    public ICollection<ForumThread> Threads { get; set; } = new List<ForumThread>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>(); 
}