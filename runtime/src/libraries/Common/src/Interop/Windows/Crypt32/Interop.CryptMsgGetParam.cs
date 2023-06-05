// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptMsgGetParam(
            SafeCryptMsgHandle hCryptMsg,
            CryptMsgParamType dwParamType,
            int dwIndex,
            out int pvData,
            ref int pcbData
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CryptMsgGetParam(
            SafeCryptMsgHandle hCryptMsg,
            CryptMsgParamType dwParamType,
            int dwIndex,
            byte* pvData,
            ref int pcbData
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptMsgGetParam(
            SafeCryptMsgHandle hCryptMsg,
            CryptMsgParamType dwParamType,
            int dwIndex,
            out CryptMsgType pvData,
            ref int pcbData
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptMsgGetParam(
            SafeCryptMsgHandle hCryptMsg,
            CryptMsgParamType dwParamType,
            int dwIndex,
            IntPtr pvData,
            ref int pcbData
        );
    }
}
