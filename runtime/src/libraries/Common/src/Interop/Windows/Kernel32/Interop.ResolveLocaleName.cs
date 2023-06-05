// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static unsafe class Kernel32
    {
        internal const int LOCALE_NAME_MAX_LENGTH = 85;

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        partial internal static int ResolveLocaleName(
            string lpNameToResolve,
            char* lpLocaleName,
            int cchLocaleName
        );
    }
}
