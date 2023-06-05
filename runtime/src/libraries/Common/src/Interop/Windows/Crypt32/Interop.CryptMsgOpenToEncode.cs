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
        partial internal static unsafe SafeCryptMsgHandle CryptMsgOpenToEncode(
            MsgEncodingType dwMsgEncodingType,
            int dwFlags,
            CryptMsgType dwMsgType,
            CMSG_ENVELOPED_ENCODE_INFO* pvMsgEncodeInfo,
            [MarshalAs(UnmanagedType.LPStr)] string pszInnerContentObjID,
            IntPtr pStreamInfo
        );
    }
}
