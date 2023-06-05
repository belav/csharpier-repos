// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class User32
    {
        [LibraryImport(Libraries.User32, EntryPoint = "SendMessageTimeoutW")]
        partial public static unsafe IntPtr SendMessageTimeout(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            int flags,
            int timeout,
            IntPtr* pdwResult
        );
    }
}
