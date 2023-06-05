// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetSockOpt")]
        partial internal static unsafe Error GetSockOpt(
            SafeHandle socket,
            SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            byte* optionValue,
            int* optionLen
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetSockOpt")]
        partial internal static unsafe Error GetSockOpt(
            IntPtr socket,
            SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            byte* optionValue,
            int* optionLen
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetRawSockOpt")]
        partial internal static unsafe Error GetRawSockOpt(
            SafeHandle socket,
            int optionLevel,
            int optionName,
            byte* optionValue,
            int* optionLen
        );
    }
}
