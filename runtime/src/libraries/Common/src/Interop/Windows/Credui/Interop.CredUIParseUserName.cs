// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Credui
    {
        // These constants were taken from the wincred.h file
        internal const int CRED_MAX_USERNAME_LENGTH = 514;
        internal const int CRED_MAX_DOMAIN_TARGET_LENGTH = 338;

        [LibraryImport(
            Libraries.Credui,
            EntryPoint = "CredUIParseUserNameW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int CredUIParseUserName(
            string pszUserName,
            char* pszUser,
            uint ulUserMaxChars,
            char* pszDomain,
            uint ulDomainMaxChars
        );
    }
}
