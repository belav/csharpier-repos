// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32)]
#if !NO_SUPPRESS_GC_TRANSITION
        [SuppressGCTransition]
#endif
        partial
#endif
        internal static IntPtr GetStdHandle(int nStdHandle); // param is NOT a handle, but it returns one!
    }
}
