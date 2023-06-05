// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_PReadV",
            SetLastError = true
        )]
        partial internal static unsafe long PReadV(
            SafeHandle fd,
            IOVector* vectors,
            int vectorCount,
            long fileOffset
        );
    }
}
