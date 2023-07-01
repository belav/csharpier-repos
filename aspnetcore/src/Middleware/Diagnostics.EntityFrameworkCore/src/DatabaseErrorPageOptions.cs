using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Options for the <see cref="DatabaseErrorPageMiddleware"/>.
/// </summary>
public class DatabaseErrorPageOptions
{
    /// <summary>
    /// Gets or sets the path that <see cref="MigrationsEndPointMiddleware"/> will listen
    /// for requests to execute migrations commands.
    /// </summary>
    public virtual PathString MigrationsEndPointPath { get; set; } =
        MigrationsEndPointOptions.DefaultPath;
}
