// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [Flags]
        internal enum SocketEvents : int
        {
            None = 0x00,
            Read = 0x01,
            Write = 0x02,
            ReadClose = 0x04,
            Close = 0x08,
            Error = 0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocketEvent
        {
            public IntPtr Data;
            public SocketEvents Events;
            private int _padding;
        }

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_CreateSocketEventPort")]
        partial internal static unsafe Error CreateSocketEventPort(IntPtr* port);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_CloseSocketEventPort")]
        partial internal static Error CloseSocketEventPort(IntPtr port);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_CreateSocketEventBuffer")]
        partial internal static unsafe Error CreateSocketEventBuffer(
            int count,
            SocketEvent** buffer
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_FreeSocketEventBuffer")]
        partial internal static unsafe Error FreeSocketEventBuffer(SocketEvent* buffer);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_TryChangeSocketEventRegistration"
        )]
        partial internal static Error TryChangeSocketEventRegistration(
            IntPtr port,
            SafeHandle socket,
            SocketEvents currentEvents,
            SocketEvents newEvents,
            IntPtr data
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_TryChangeSocketEventRegistration"
        )]
        partial internal static Error TryChangeSocketEventRegistration(
            IntPtr port,
            IntPtr socket,
            SocketEvents currentEvents,
            SocketEvents newEvents,
            IntPtr data
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_WaitForSocketEvents")]
        partial internal static unsafe Error WaitForSocketEvents(
            IntPtr port,
            SocketEvent* buffer,
            int* count
        );
    }
}
