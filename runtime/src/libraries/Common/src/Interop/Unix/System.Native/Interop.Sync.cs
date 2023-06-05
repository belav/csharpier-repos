// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        /// <summary>
        /// Forces a write of all modified I/O buffers to their storage mediums.
        /// </summary>
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_Sync")]
        partial internal static void Sync();
    }
}
