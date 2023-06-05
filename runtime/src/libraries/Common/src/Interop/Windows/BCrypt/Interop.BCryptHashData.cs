// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        [LibraryImport(Libraries.BCrypt, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static NTSTATUS BCryptHashData(
            SafeBCryptHashHandle hHash,
            ReadOnlySpan<byte> pbInput,
            int cbInput,
            int dwFlags
        );
    }
}
