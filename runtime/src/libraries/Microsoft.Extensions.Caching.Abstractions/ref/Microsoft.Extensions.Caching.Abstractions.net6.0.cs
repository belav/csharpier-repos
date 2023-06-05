// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Extensions.Caching.Memory
{
    partial public interface IMemoryCache : System.IDisposable
    {
        MemoryCacheStatistics? GetCurrentStatistics() => null;
    }
}
