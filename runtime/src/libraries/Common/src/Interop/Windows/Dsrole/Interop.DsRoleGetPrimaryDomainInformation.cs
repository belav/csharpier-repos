// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Dsrole
    {
        internal enum DSROLE_PRIMARY_DOMAIN_INFO_LEVEL
        {
            DsRolePrimaryDomainInfoBasic = 1,
            DsRoleUpgradeStatus = 2,
            DsRoleOperationState = 3,
            DsRolePrimaryDomainInfoBasicEx = 4
        }

        [LibraryImport(Libraries.Dsrole, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static int DsRoleGetPrimaryDomainInformation(
            [MarshalAs(UnmanagedType.LPTStr)] string lpServer,
            DSROLE_PRIMARY_DOMAIN_INFO_LEVEL InfoLevel,
            out IntPtr Buffer
        );
    }
}
