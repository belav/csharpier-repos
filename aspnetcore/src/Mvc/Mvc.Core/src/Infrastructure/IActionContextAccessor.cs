using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Mvc.Infrastructure;

/// <summary>
/// Defines an interface for exposing an <see cref="ActionContext"/>.
/// </summary>
public interface IActionContextAccessor
{
    /// <summary>
    /// Gets or sets the <see cref="ActionContext"/>.
    /// </summary>
    [DisallowNull]
    ActionContext? ActionContext { get; set; }
}
