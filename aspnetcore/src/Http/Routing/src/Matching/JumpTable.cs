using System.Diagnostics;

namespace Microsoft.AspNetCore.Routing.Matching;

[DebuggerDisplay("{DebuggerToString(),nq}")]
internal abstract class JumpTable
{
    public abstract int GetDestination(string path, PathSegment segment);

    public virtual string DebuggerToString()
    {
        return GetType().Name;
    }
}
