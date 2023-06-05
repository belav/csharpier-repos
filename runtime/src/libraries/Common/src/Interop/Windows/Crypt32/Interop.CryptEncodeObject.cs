// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        internal static unsafe bool CryptEncodeObject(
            MsgEncodingType dwCertEncodingType,
            CryptDecodeObjectStructType lpszStructType,
            void* pvStructInfo,
            byte[]? pbEncoded,
            ref int pcbEncoded
        )
        {
            return CryptEncodeObject(
                dwCertEncodingType,
                (nint)lpszStructType,
                pvStructInfo,
                pbEncoded,
                ref pcbEncoded
            );
        }

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static unsafe bool CryptEncodeObject(
            MsgEncodingType dwCertEncodingType,
            nint lpszStructType,
            void* pvStructInfo,
            byte[]? pbEncoded,
            ref int pcbEncoded
        );
    }
}
