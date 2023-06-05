// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Sockets;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Winsock
    {
        [LibraryImport(
            Interop.Libraries.Ws2_32,
            EntryPoint = "WSADuplicateSocketW",
            SetLastError = true
        )]
        partial internal static unsafe int WSADuplicateSocket(
            SafeSocketHandle s,
            uint dwProcessId,
            WSAPROTOCOL_INFOW* lpProtocolInfo
        );
    }
}
