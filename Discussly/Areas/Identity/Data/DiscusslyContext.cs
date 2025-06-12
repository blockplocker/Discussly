using Discussly.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Discussly.Models;

namespace Discussly.Data;

public class DiscusslyContext : IdentityDbContext<DiscusslyUser>
{
    public DiscusslyContext(DbContextOptions<DiscusslyContext> options)
        : base(options)
    {
    }

    public DbSet<Report> Reports { get; set; } = default!;
    public DbSet<PrivateMessage> PrivateMessages { get; set; } = default!;
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

}
