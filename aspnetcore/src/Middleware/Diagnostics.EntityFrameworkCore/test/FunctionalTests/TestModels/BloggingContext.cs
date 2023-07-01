using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Tests;

public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Blog> Blogs { get; set; }
}
