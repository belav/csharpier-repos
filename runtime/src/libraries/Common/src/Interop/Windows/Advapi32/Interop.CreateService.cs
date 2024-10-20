// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Advapi32
    {
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "CreateServiceW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        public static partial IntPtr CreateService(
            SafeServiceHandle databaseHandle,
            string serviceName,
            string displayName,
            int access,
            int serviceType,
            int startType,
            int errorControl,
            string binaryPath,
            string loadOrderGroup,
            IntPtr pTagId,
            string dependencies,
            string servicesStartName,
            string password
        );
    }
}
