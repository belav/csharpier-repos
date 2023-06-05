// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
    partial internal static class StorePal
    {
#pragma warning disable IDE0060
        partial
#pragma warning disable IDE0060
        internal static IStorePal FromHandle(IntPtr storeHandle)
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static ILoaderPal FromBlob(
            ReadOnlySpan<byte> rawData,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static ILoaderPal FromFile(
            string fileName,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static IExportPal FromCertificate(ICertificatePalCore cert)
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static IExportPal LinkFromCertificateCollection(
            X509Certificate2Collection certificates
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static IStorePal FromSystemStore(
            string storeName,
            StoreLocation storeLocation,
            OpenFlags openFlags
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }
#pragma warning restore IDE0060
    }
}
