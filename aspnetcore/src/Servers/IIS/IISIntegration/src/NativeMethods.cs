// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Server.IISIntegration;

partial internal static class NativeMethods
{
    private const string KERNEL32 = "kernel32.dll";

    [LibraryImport(KERNEL32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    partial public static bool CloseHandle(IntPtr handle);
}
