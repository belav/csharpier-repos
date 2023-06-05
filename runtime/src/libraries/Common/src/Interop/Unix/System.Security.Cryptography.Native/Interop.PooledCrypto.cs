// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Security.Cryptography;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        internal static ArraySegment<byte> RentAsn1StringBytes(IntPtr asn1)
        {
            return RentDynamicBuffer(GetAsn1StringBytes, asn1);
        }

        private static ArraySegment<byte> RentDynamicBuffer<THandle>(
            NegativeSizeReadMethod<THandle> method,
            THandle handle
        )
        {
            int negativeSize = method(handle, null, 0);

            if (negativeSize > 0)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            int targetSize = -negativeSize;
            byte[] bytes = CryptoPool.Rent(targetSize);

            int ret = method(handle, bytes, targetSize);

            if (ret != 1)
            {
                CryptoPool.Return(bytes);
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            return new ArraySegment<byte>(bytes, 0, targetSize);
        }
    }
}
