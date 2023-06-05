// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class CoreFoundation
    {
        [LibraryImport(Libraries.CoreFoundationLibrary)]
        partial private static SafeCreateHandle CFURLCreateWithString(
            IntPtr allocator,
            SafeCreateHandle str,
            IntPtr baseUrl
        );

        internal static SafeCreateHandle CFURLCreateWithString(string url)
        {
            Debug.Assert(url != null);
            using (SafeCreateHandle stringHandle = CFStringCreateWithCString(url))
            {
                return CFURLCreateWithString(IntPtr.Zero, stringHandle, IntPtr.Zero);
            }
        }
    }
}
