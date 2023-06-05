// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        internal delegate int NegativeSizeReadMethod<in THandle>(
            THandle handle,
            byte[]? buf,
            int cBuf
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioTell")]
        partial internal static int CryptoNative_BioTell(SafeBioHandle bio);

        internal static int BioTell(SafeBioHandle bio)
        {
            int ret = CryptoNative_BioTell(bio);
            if (ret < 0)
            {
                throw CreateOpenSslCryptographicException();
            }

            return ret;
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioSeek")]
        partial internal static int BioSeek(SafeBioHandle bio, int pos);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509Thumbprint")]
        partial private static int GetX509Thumbprint(SafeX509Handle x509, byte[]? buf, int cBuf);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509NameRawBytes")]
        partial private static int GetX509NameRawBytes(IntPtr x509Name, byte[]? buf, int cBuf);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_ReadX509AsDerFromBio")]
        partial internal static SafeX509Handle ReadX509AsDerFromBio(SafeBioHandle bio);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509CrlNextUpdate")]
        partial internal static IntPtr GetX509CrlNextUpdate(SafeX509CrlHandle crl);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509Version")]
        partial internal static int GetX509Version(SafeX509Handle x509);

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_GetX509PublicKeyParameterBytes"
        )]
        partial private static int GetX509PublicKeyParameterBytes(
            SafeX509Handle x509,
            byte[]? buf,
            int cBuf
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509EkuFieldCount")]
        partial internal static int GetX509EkuFieldCount(SafeEkuExtensionHandle eku);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509EkuField")]
        partial internal static IntPtr GetX509EkuField(SafeEkuExtensionHandle eku, int loc);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509NameInfo")]
        partial internal static SafeBioHandle GetX509NameInfo(
            SafeX509Handle x509,
            int nameType,
            [MarshalAs(UnmanagedType.Bool)] bool forIssuer
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetAsn1StringBytes")]
        partial private static int GetAsn1StringBytes(IntPtr asn1, byte[]? buf, int cBuf);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_PushX509StackField")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool PushX509StackField(
            SafeX509StackHandle stack,
            SafeX509Handle x509
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_PushX509StackField")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool PushX509StackField(
            SafeSharedX509StackHandle stack,
            SafeX509Handle x509
        );

        internal static unsafe string? GetX509RootStorePath(out bool defaultPath)
        {
            byte usedDefault;
            IntPtr ptr = GetX509RootStorePath_private(&usedDefault);
            defaultPath = (usedDefault != 0);
            return Marshal.PtrToStringUTF8(ptr);
        }

        internal static unsafe string? GetX509RootStoreFile()
        {
            byte unused;
            return Marshal.PtrToStringUTF8(GetX509RootStoreFile_private(&unused));
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509RootStorePath")]
        partial private static unsafe IntPtr GetX509RootStorePath_private(byte* defaultPath);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509RootStoreFile")]
        partial private static unsafe IntPtr GetX509RootStoreFile_private(byte* defaultPath);

        [LibraryImport(Libraries.CryptoNative)]
        partial private static int CryptoNative_X509StoreSetVerifyTime(
            SafeX509StoreHandle ctx,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            [MarshalAs(UnmanagedType.Bool)] bool isDst
        );

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_CheckX509IpAddress",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static int CheckX509IpAddress(
            SafeX509Handle x509,
            byte[] addressBytes,
            int addressLen,
            string hostname,
            int cchHostname
        );

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_CheckX509Hostname",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static int CheckX509Hostname(
            SafeX509Handle x509,
            string hostname,
            int cchHostname
        );

        internal static byte[] GetAsn1StringBytes(IntPtr asn1)
        {
            return GetDynamicBuffer(GetAsn1StringBytes, asn1);
        }

        internal static byte[] GetX509Thumbprint(SafeX509Handle x509)
        {
            return GetDynamicBuffer(GetX509Thumbprint, x509);
        }

        internal static X500DistinguishedName LoadX500Name(IntPtr namePtr)
        {
            CheckValidOpenSslHandle(namePtr);

            byte[] buf = GetDynamicBuffer(GetX509NameRawBytes, namePtr);
            return new X500DistinguishedName(buf);
        }

        internal static byte[] GetX509PublicKeyParameterBytes(SafeX509Handle x509)
        {
            return GetDynamicBuffer(GetX509PublicKeyParameterBytes, x509);
        }

        internal static void X509StoreSetVerifyTime(SafeX509StoreHandle ctx, DateTime verifyTime)
        {
            // OpenSSL is going to convert our input time to universal, so we should be in Local or
            // Unspecified (local-assumed).
            Debug.Assert(
                verifyTime.Kind != DateTimeKind.Utc,
                "UTC verifyTime should have been normalized to Local"
            );

            int succeeded = CryptoNative_X509StoreSetVerifyTime(
                ctx,
                verifyTime.Year,
                verifyTime.Month,
                verifyTime.Day,
                verifyTime.Hour,
                verifyTime.Minute,
                verifyTime.Second,
                verifyTime.IsDaylightSavingTime()
            );

            if (succeeded != 1)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }

        internal static byte[] GetDynamicBuffer<THandle>(
            NegativeSizeReadMethod<THandle> method,
            THandle handle
        )
        {
            int negativeSize = method(handle, null, 0);

            if (negativeSize > 0)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            byte[] bytes = new byte[-negativeSize];

            int ret = method(handle, bytes, bytes.Length);

            if (ret != 1)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            return bytes;
        }
    }
}
