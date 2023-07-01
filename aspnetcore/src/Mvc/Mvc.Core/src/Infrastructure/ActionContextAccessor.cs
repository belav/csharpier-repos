using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Mvc.Infrastructure;

/// <summary>
/// Type that provides access to an <see cref="ActionContext"/>.
/// </summary>
public class ActionContextAccessor : IActionContextAccessor
{
    internal static readonly IActionContextAccessor Null = new NullActionContextAccessor();

    private static readonly AsyncLocal<ActionContext> _storage = new AsyncLocal<ActionContext>();

    /// <inheritdoc/>
    [DisallowNull]
    public ActionContext? ActionContext
    {
        get { return _storage.Value; }
        set { _storage.Value = value; }
    }

    private sealed class NullActionContextAccessor : IActionContextAccessor
    {
        public ActionContext? ActionContext
        {
            get => null;
            set { }
        }
    }
}
