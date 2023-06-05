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
        internal enum CFNumberType
        {
            kCFNumberIntType = 9,
        }

        [LibraryImport(Libraries.CoreFoundationLibrary)]
        partial private static unsafe int CFNumberGetValue(
            IntPtr handle,
            CFNumberType type,
            int* value
        );
    }
}
