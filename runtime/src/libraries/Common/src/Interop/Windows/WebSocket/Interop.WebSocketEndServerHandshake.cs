// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class WebSocket
    {
        [LibraryImport(Libraries.WebSocket)]
        partial internal static int WebSocketEndServerHandshake(SafeHandle webSocketHandle);
    }
}
