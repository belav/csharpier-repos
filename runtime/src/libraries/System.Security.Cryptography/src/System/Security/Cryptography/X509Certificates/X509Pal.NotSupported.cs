// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography.X509Certificates
{
    partial internal static class X509Pal
    {
        partial private static IX509Pal BuildSingleton()
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }
    }
}
