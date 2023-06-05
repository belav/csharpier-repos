// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Sockets;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetSocketType")]
        partial internal static Error GetSocketType(
            SafeSocketHandle socket,
            out AddressFamily addressFamily,
            out SocketType socketType,
            out ProtocolType protocolType,
            [MarshalAs(UnmanagedType.Bool)] out bool isListening
        );
    }
}
