// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Winspool
    {
        [LibraryImport(
            Libraries.Winspool,
            EntryPoint = "DeviceCapabilitiesW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int DeviceCapabilities(
            string pDevice,
            string pPort,
            short fwCapabilities,
            IntPtr pOutput,
            IntPtr /*DEVMODE*/
            pDevMode
        );
    }
}
