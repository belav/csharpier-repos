// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Text;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetIPSocketAddressSizes")]
        [SuppressGCTransition]
        partial internal static unsafe Error GetIPSocketAddressSizes(
            int* ipv4SocketAddressSize,
            int* ipv6SocketAddressSize
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetAddressFamily")]
        [SuppressGCTransition]
        partial internal static unsafe Error GetAddressFamily(
            byte* socketAddress,
            int socketAddressLen,
            int* addressFamily
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetAddressFamily")]
        [SuppressGCTransition]
        partial internal static unsafe Error SetAddressFamily(
            byte* socketAddress,
            int socketAddressLen,
            int addressFamily
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetPort")]
        [SuppressGCTransition]
        partial internal static unsafe Error GetPort(
            byte* socketAddress,
            int socketAddressLen,
            ushort* port
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetPort")]
        [SuppressGCTransition]
        partial internal static unsafe Error SetPort(
            byte* socketAddress,
            int socketAddressLen,
            ushort port
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetIPv4Address")]
        [SuppressGCTransition]
        partial internal static unsafe Error GetIPv4Address(
            byte* socketAddress,
            int socketAddressLen,
            uint* address
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetIPv4Address")]
        [SuppressGCTransition]
        partial internal static unsafe Error SetIPv4Address(
            byte* socketAddress,
            int socketAddressLen,
            uint address
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetIPv6Address")]
        partial internal static unsafe Error GetIPv6Address(
            byte* socketAddress,
            int socketAddressLen,
            byte* address,
            int addressLen,
            uint* scopeId
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetIPv6Address")]
        partial internal static unsafe Error SetIPv6Address(
            byte* socketAddress,
            int socketAddressLen,
            byte* address,
            int addressLen,
            uint scopeId
        );
    }
}
