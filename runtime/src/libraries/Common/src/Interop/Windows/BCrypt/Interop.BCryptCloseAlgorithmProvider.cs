// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        [LibraryImport(Libraries.BCrypt)]
        partial internal static NTSTATUS BCryptCloseAlgorithmProvider(
            IntPtr hAlgorithm,
            int dwFlags
        );
    }
}
