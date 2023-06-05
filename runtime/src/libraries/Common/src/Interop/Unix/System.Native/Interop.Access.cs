// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal enum AccessMode : int
        {
            F_OK = 0, /* Check for existence */
            X_OK = 1, /* Check for execute */
            W_OK = 2, /* Check for write */
            R_OK = 4, /* Check for read */
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_Access",
            StringMarshalling = StringMarshalling.Utf8,
            SetLastError = true
        )]
        partial internal static int Access(string path, AccessMode mode);
    }
}
