// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class AppleCrypto
    {
        [LibraryImport(Libraries.AppleCryptoNative)]
        private static partial SafeCFStringHandle AppleCryptoNative_SecCopyErrorMessageString(int osStatus);

        internal static string? GetSecErrorString(int osStatus)
        {
            using (SafeCFStringHandle cfString = AppleCryptoNative_SecCopyErrorMessageString(osStatus))
            {
                if (cfString.IsInvalid)
                {
                    return null;
                }

                return CoreFoundation.CFStringToString(cfString);
            }
        }
    }
}
