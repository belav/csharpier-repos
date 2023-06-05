// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security;
using static System.Net.WebSockets.WebSocketProtocolComponent;

partial internal static class Interop
{
    partial internal static class WebSocket
    {
        [LibraryImport(Libraries.WebSocket, EntryPoint = "WebSocketSend")]
        partial internal static int WebSocketSend_Raw(
            SafeHandle webSocketHandle,
            BufferType bufferType,
            ref Buffer buffer,
            IntPtr applicationContext
        );

        [LibraryImport(Libraries.WebSocket, EntryPoint = "WebSocketSend")]
        partial internal static int WebSocketSendWithoutBody_Raw(
            SafeHandle webSocketHandle,
            BufferType bufferType,
            IntPtr buffer,
            IntPtr applicationContext
        );
    }
}
