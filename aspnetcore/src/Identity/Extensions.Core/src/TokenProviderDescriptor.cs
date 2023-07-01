using System;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Used to represents a token provider in <see cref="TokenOptions"/>'s TokenMap.
/// </summary>
public class TokenProviderDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenProviderDescriptor"/> class.
    /// </summary>
    /// <param name="type">The concrete type for this token provider.</param>
    public TokenProviderDescriptor(Type type)
    {
        ProviderType = type;
    }

    /// <summary>
    /// The type that will be used for this token provider.
    /// </summary>
    public Type ProviderType { get; }

    /// <summary>
    /// If specified, the instance to be used for the token provider.
    /// </summary>
    public object? ProviderInstance { get; set; }
}
