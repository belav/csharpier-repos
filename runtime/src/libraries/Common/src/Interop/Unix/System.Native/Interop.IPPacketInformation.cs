// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial
#pragma warning disable CA1823 // unused private padding fields in MulticastOption structs

internal static class Interop
{
    partial internal static class Sys
    {
        internal struct IPPacketInformation
        {
            public IPAddress Address; // Destination IP Address
            public int InterfaceIndex; // Interface index
            private int _padding; // Pad out to 8-byte alignment
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetControlMessageBufferSize"
        )]
        [SuppressGCTransition]
        partial internal static int GetControlMessageBufferSize(int isIPv4, int isIPv6);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_TryGetIPPacketInformation"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool TryGetIPPacketInformation(
            MessageHeader* messageHeader,
            [MarshalAs(UnmanagedType.Bool)] bool isIPv4,
            IPPacketInformation* packetInfo
        );
    }
}
