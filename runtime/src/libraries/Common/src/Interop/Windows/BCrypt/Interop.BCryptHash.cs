// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        [LibraryImport(Libraries.BCrypt, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static unsafe NTSTATUS BCryptHash(
            nuint hAlgorithm,
            byte* pbSecret,
            int cbSecret,
            byte* pbInput,
            int cbInput,
            byte* pbOutput,
            int cbOutput
        );
    }
}
