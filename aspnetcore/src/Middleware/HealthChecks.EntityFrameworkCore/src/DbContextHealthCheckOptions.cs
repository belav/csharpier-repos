using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

internal sealed class DbContextHealthCheckOptions<TContext>
    where TContext : DbContext
{
    public Func<TContext, CancellationToken, Task<bool>>? CustomTestQuery { get; set; }
}
