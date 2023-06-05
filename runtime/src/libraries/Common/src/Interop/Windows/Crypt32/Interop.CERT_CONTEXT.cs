// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CERT_CONTEXT
        {
            internal MsgEncodingType dwCertEncodingType;
            internal byte* pbCertEncoded;
            internal int cbCertEncoded;
            internal CERT_INFO* pCertInfo;
            internal IntPtr hCertStore;
        }
    }
}
