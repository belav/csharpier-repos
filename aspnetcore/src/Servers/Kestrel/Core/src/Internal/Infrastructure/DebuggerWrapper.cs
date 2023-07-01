using System.Diagnostics;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

internal sealed class DebuggerWrapper : IDebugger
{
    private DebuggerWrapper() { }

    public static IDebugger Singleton { get; } = new DebuggerWrapper();

    public bool IsAttached => Debugger.IsAttached;
}
