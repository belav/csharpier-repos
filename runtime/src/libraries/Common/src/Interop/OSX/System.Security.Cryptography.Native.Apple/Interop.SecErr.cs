// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

partial internal static class Interop
{
    partial internal static class AppleCrypto
    {
        internal static Exception CreateExceptionForOSStatus(int osStatus)
        {
            string? msg = GetSecErrorString(osStatus);

            if (msg == null)
            {
                return CreateExceptionForCCError(osStatus, "OSStatus");
            }

            return new AppleCommonCryptoCryptographicException(osStatus, msg);
        }
    }
}
