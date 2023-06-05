// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        internal static unsafe bool GetRandomBytes(byte* pbBuffer, int count)
        {
            Debug.Assert(count >= 0);

            return CryptoNative_GetRandomBytes(pbBuffer, count);
        }

        [LibraryImport(Libraries.AndroidCryptoNative)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static unsafe bool CryptoNative_GetRandomBytes(byte* buf, int num);
    }
}
