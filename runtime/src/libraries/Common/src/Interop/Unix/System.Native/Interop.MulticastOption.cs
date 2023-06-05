// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial
#pragma warning disable CA1823 // unused private padding fields in MulticastOption structs

internal static class Interop
{
    partial internal static class Sys
    {
        internal enum MulticastOption : int
        {
            MULTICAST_ADD = 0,
            MULTICAST_DROP = 1,
            MULTICAST_IF = 2
        }

        internal struct IPv4MulticastOption
        {
            public uint MulticastAddress;
            public uint LocalAddress;
            public int InterfaceIndex;
            private int _padding;
        }

        internal struct IPv6MulticastOption
        {
            public IPAddress Address;
            public int InterfaceIndex;
            private int _padding;
        }

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetIPv4MulticastOption")]
        partial internal static unsafe Error GetIPv4MulticastOption(
            SafeHandle socket,
            MulticastOption multicastOption,
            IPv4MulticastOption* option
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetIPv4MulticastOption")]
        partial internal static unsafe Error SetIPv4MulticastOption(
            SafeHandle socket,
            MulticastOption multicastOption,
            IPv4MulticastOption* option
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetIPv6MulticastOption")]
        partial internal static unsafe Error GetIPv6MulticastOption(
            SafeHandle socket,
            MulticastOption multicastOption,
            IPv6MulticastOption* option
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetIPv6MulticastOption")]
        partial internal static unsafe Error SetIPv6MulticastOption(
            SafeHandle socket,
            MulticastOption multicastOption,
            IPv6MulticastOption* option
        );
    }
}
