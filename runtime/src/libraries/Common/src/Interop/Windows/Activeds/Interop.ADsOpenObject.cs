// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Activeds
    {
        [LibraryImport(Interop.Libraries.Activeds, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static int ADsOpenObject(
            string path,
            string? userName,
            string? password,
            int flags,
            ref Guid iid,
            out IntPtr ppObject
        );
    }
}
