// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_RC2_CBC_PARAMETERS
        {
            internal CryptRc2Version dwVersion;
            internal int fIV;
            internal byte rgbIV0;
            internal byte rgbIV1;
            internal byte rgbIV2;
            internal byte rgbIV3;
            internal byte rgbIV4;
            internal byte rgbIV5;
            internal byte rgbIV6;
            internal byte rgbIV7;
        }
    }
}
