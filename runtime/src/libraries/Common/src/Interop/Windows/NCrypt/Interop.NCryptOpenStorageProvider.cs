// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class NCrypt
    {
        [LibraryImport(Interop.Libraries.NCrypt, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static ErrorCode NCryptOpenStorageProvider(
            out SafeNCryptProviderHandle phProvider,
            string pszProviderName,
            int dwFlags
        );
    }
}
