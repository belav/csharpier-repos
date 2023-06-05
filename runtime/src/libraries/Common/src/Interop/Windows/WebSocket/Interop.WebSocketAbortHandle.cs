// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security;

partial internal static class Interop
{
    partial internal static class WebSocket
    {
        [LibraryImport(Libraries.WebSocket)]
        partial internal static void WebSocketAbortHandle(SafeHandle webSocketHandle);
    }
}
