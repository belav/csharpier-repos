// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Net.Internals;
using System.Net.Sockets;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_Socket")]
        partial internal static unsafe Error Socket(
            AddressFamily addressFamily,
            SocketType socketType,
            ProtocolType protocolType,
            IntPtr* socket
        );
    }
}
