// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct IPEndPointInfo
        {
            fixed public byte AddressBytes[16];
            public uint NumAddressBytes;
            public uint Port;
            private uint __padding; // For native struct-size padding. Does not contain useful data.
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeTcpConnectionInformation
        {
            public IPEndPointInfo LocalEndPoint;
            public IPEndPointInfo RemoteEndPoint;
            public TcpState State;
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetEstimatedTcpConnectionCount"
        )]
        partial public static int GetEstimatedTcpConnectionCount();

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetActiveTcpConnectionInfos"
        )]
        partial public static unsafe int GetActiveTcpConnectionInfos(
            NativeTcpConnectionInformation* infos,
            int* infoCount
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_GetEstimatedUdpListenerCount"
        )]
        partial public static int GetEstimatedUdpListenerCount();

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetActiveUdpListeners")]
        partial public static unsafe int GetActiveUdpListeners(
            IPEndPointInfo* infos,
            int* infoCount
        );
    }
}
