using Microsoft.EntityFrameworkCore;

namespace BasicViews
{
    public class BasicViewsContext : DbContext
    {
        public BasicViewsContext(DbContextOptions options)
            : base(options) { }

        public virtual DbSet<Person> People { get; set; }
    }
}
