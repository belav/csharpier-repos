// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [Flags]
        internal enum HandleFlags : uint
        {
            None = 0,
            HANDLE_FLAG_INHERIT = 1,
            HANDLE_FLAG_PROTECT_FROM_CLOSE = 2
        }

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool SetHandleInformation(
            SafeHandle hObject,
            HandleFlags dwMask,
            HandleFlags dwFlags
        );
    }
}
