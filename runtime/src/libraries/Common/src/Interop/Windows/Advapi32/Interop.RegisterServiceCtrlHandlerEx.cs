// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        public delegate int ServiceControlCallbackEx(
            int control,
            int eventType,
            IntPtr eventData,
            IntPtr eventContext
        );

        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "RegisterServiceCtrlHandlerExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial public static IntPtr RegisterServiceCtrlHandlerEx(
            string? serviceName,
            ServiceControlCallbackEx? callback,
            IntPtr userData
        );
    }
}
