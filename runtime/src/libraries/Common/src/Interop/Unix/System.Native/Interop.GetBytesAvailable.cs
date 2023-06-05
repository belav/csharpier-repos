// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetBytesAvailable")]
        partial internal static unsafe Error GetBytesAvailable(SafeHandle socket, int* available);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetAtOutOfBandMark")]
        partial internal static unsafe Error GetAtOutOfBandMark(SafeHandle socket, int* atMark);
    }
}
