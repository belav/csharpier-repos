// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
    partial internal class StorePal
    {
        partial internal static IStorePal FromHandle(IntPtr storeHandle);

        partial internal static ILoaderPal FromBlob(
            ReadOnlySpan<byte> rawData,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        );

        partial internal static ILoaderPal FromFile(
            string fileName,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        );

        partial internal static IExportPal FromCertificate(ICertificatePalCore cert);

        partial internal static IExportPal LinkFromCertificateCollection(
            X509Certificate2Collection certificates
        );

        partial internal static IStorePal FromSystemStore(
            string storeName,
            StoreLocation storeLocation,
            OpenFlags openFlags
        );
    }
}
