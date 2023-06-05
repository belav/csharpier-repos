// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

partial internal static class Interop
{
    partial internal static class Winsock
    {
        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static SocketError WSAConnect(
            SafeSocketHandle socketHandle,
            byte[] socketAddress,
            int socketAddressSize,
            IntPtr inBuffer,
            IntPtr outBuffer,
            IntPtr sQOS,
            IntPtr gQOS
        );
    }
}
