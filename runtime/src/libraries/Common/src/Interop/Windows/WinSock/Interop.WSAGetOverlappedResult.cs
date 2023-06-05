// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Threading;

partial internal static class Interop
{
    partial internal static class Winsock
    {
        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool WSAGetOverlappedResult(
            SafeSocketHandle socketHandle,
            NativeOverlapped* overlapped,
            out uint bytesTransferred,
            [MarshalAs(UnmanagedType.Bool)] bool wait,
            out SocketFlags socketFlags
        );
    }
}
