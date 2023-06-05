// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CRYPT_KEY_PROV_INFO
        {
            internal char* pwszContainerName;
            internal char* pwszProvName;
            internal int dwProvType;
            internal CryptAcquireContextFlags dwFlags;
            internal int cProvParam;
            internal IntPtr rgProvParam;
            internal int dwKeySpec;
        }
    }
}
