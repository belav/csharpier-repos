// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptMsgControl(
            SafeCryptMsgHandle hCryptMsg,
            int dwFlags,
            MsgControlType dwCtrlType,
            ref CMSG_CTRL_DECRYPT_PARA pvCtrlPara
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptMsgControl(
            SafeCryptMsgHandle hCryptMsg,
            int dwFlags,
            MsgControlType dwCtrlType,
            ref CMSG_CTRL_KEY_AGREE_DECRYPT_PARA pvCtrlPara
        );
    }
}
