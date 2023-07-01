using System.Runtime.Serialization;

namespace Microsoft.AspNetCore.Routing.Matching;

/// <summary>
/// An exception which indicates multiple matches in endpoint selection.
/// </summary>
[Serializable]
internal sealed class AmbiguousMatchException : Exception
{
    public AmbiguousMatchException(string message)
        : base(message) { }

    internal AmbiguousMatchException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
