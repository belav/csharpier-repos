// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [Flags]
        internal enum GetNameInfoFlags : int
        {
            NI_NAMEREQD = 0x1,
            NI_NUMERICHOST = 0x2,
        }

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetNameInfo")]
        partial internal static unsafe int GetNameInfo(
            byte* address,
            uint addressLength,
            byte isIpv6,
            byte* host,
            uint hostLength,
            byte* service,
            uint serviceLength,
            GetNameInfoFlags flags
        );
    }
}
