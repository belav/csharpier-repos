using System;
using Microsoft.AspNetCore.Razor.Language.Components;

namespace Microsoft.AspNetCore.Razor.Language;

public static class RequiredAttributeDescriptorExtensions
{
    public static bool IsDirectiveAttribute(this RequiredAttributeDescriptor descriptor)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Metadata.TryGetValue(
                ComponentMetadata.Common.DirectiveAttribute,
                out var value
            ) && string.Equals(bool.TrueString, value);
    }
}
