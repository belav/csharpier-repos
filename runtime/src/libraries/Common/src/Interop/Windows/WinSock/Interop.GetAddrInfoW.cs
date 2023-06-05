// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Winsock
    {
        [LibraryImport(
            Interop.Libraries.Ws2_32,
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int GetAddrInfoW(
            string pNameName,
            string? pServiceName,
            AddressInfo* pHints,
            AddressInfo** ppResult
        );

        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static unsafe void FreeAddrInfoW(AddressInfo* info);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct AddressInfo
        {
            internal AddressInfoHints ai_flags;
            internal AddressFamily ai_family;
            internal int ai_socktype;
            internal int ai_protocol;
            internal nuint ai_addrlen;
            internal sbyte* ai_canonname; // Ptr to the canonical name - check for NULL
            internal byte* ai_addr; // Ptr to the sockaddr structure
            internal AddressInfo* ai_next; // Ptr to the next AddressInfo structure
        }
    }
}
