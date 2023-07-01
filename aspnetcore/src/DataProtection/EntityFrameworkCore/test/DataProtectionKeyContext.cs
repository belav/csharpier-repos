using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.Test;

class DataProtectionKeyContext : DbContext, IDataProtectionKeyContext
{
    public DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options)
        : base(options) { }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}
