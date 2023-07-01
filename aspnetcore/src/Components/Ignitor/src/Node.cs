using System.Diagnostics;

namespace Ignitor;

[DebuggerDisplay("{SerializedValue}")]
public abstract class Node
{
    public virtual ContainerNode? Parent { get; set; }

    public string SerializedValue => NodeSerializer.Serialize(this);
}

#nullable restore
