using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

/// <summary>
/// Implements policy for resolving the default key from a candidate keyring.
/// </summary>
public interface IDefaultKeyResolver
{
    /// <summary>
    /// Locates the default key from the keyring.
    /// </summary>
    DefaultKeyResolution ResolveDefaultKeyPolicy(DateTimeOffset now, IEnumerable<IKey> allKeys);
}
