// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct LinkLayerAddressInfo
        {
            public int InterfaceIndex;
            fixed public byte AddressBytes[8];
            public byte NumAddressBytes;
            private byte __padding; // For native struct-size padding. Does not contain useful data.
            public ushort HardwareType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct IpAddressInfo
        {
            public int InterfaceIndex;
            fixed public byte AddressBytes[16];
            public byte NumAddressBytes;
            public byte PrefixLength;
            fixed private byte __padding[2];
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct NetworkInterfaceInfo
        {
            fixed public byte Name[16];
            public long Speed;
            public int InterfaceIndex;
            public int Mtu;
            public ushort HardwareType;
            public byte OperationalState;
            public byte NumAddressBytes;
            fixed public byte AddressBytes[8];
            public byte SupportsMulticast;
            fixed private byte __padding[3];
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_EnumerateInterfaceAddresses"
        )]
        partial public static unsafe int EnumerateInterfaceAddresses(
            void* context,
            delegate* unmanaged<void*, byte*, IpAddressInfo*, void> ipv4Found,
            delegate* unmanaged<void*, byte*, IpAddressInfo*, uint*, void> ipv6Found,
            delegate* unmanaged<void*, byte*, LinkLayerAddressInfo*, void> linkLayerFound
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_EnumerateGatewayAddressesForInterface"
        )]
        partial public static unsafe int EnumerateGatewayAddressesForInterface(
            void* context,
            uint interfaceIndex,
            delegate* unmanaged<void*, IpAddressInfo*, void> onGatewayFound
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetNetworkInterfaces",
            SetLastError = true
        )]
        partial public static unsafe int GetNetworkInterfaces(
            int* count,
            NetworkInterfaceInfo** addrs,
            int* addressCount,
            IpAddressInfo** aa
        );
    }
}
