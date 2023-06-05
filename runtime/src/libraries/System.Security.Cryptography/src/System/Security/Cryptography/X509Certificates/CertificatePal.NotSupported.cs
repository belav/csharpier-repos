// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
    partial internal static class CertificatePal
    {
#pragma warning disable IDE0060
        partial
#pragma warning disable IDE0060
        internal static ICertificatePal FromHandle(IntPtr handle)
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static ICertificatePal FromOtherCert(X509Certificate copyFrom)
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static ICertificatePal FromBlob(
            ReadOnlySpan<byte> rawData,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static ICertificatePal FromFile(
            string fileName,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }
#pragma warning restore IDE0060
    }
}
