// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Net.Sockets;

partial internal static class Interop
{
    partial internal static class Winsock
    {
        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static unsafe SocketError getsockopt(
            SafeSocketHandle socketHandle,
            SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            byte* optionValue,
            ref int optionLength
        );

        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static SocketError getsockopt(
            SafeSocketHandle socketHandle,
            SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            out Linger optionValue,
            ref int optionLength
        );

        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static SocketError getsockopt(
            SafeSocketHandle socketHandle,
            SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            out IPMulticastRequest optionValue,
            ref int optionLength
        );

        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static SocketError getsockopt(
            SafeSocketHandle socketHandle,
            SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            out IPv6MulticastRequest optionValue,
            ref int optionLength
        );
    }
}
