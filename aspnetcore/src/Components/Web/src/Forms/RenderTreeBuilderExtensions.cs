using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Components.Rendering;

internal static class RenderTreeBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddAttributeIfNotNullOrEmpty(
        this RenderTreeBuilder builder,
        int sequence,
        string name,
        string? value
    )
    {
        if (!string.IsNullOrEmpty(value))
        {
            builder.AddAttribute(sequence, name, value);
        }
    }
}
