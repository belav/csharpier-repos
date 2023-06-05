// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System;

partial internal static class Interop
{
    partial internal static class Sys
    {
        public enum NetworkChangeKind
        {
            None = -1,
            AddressAdded = 0,
            AddressRemoved = 1,
            AvailabilityChanged = 2
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_CreateNetworkChangeListenerSocket",
            SetLastError = true
        )]
        partial public static unsafe Error CreateNetworkChangeListenerSocket(IntPtr* socket);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_ReadEvents")]
        partial public static unsafe Error ReadEvents(
            SafeHandle socket,
            delegate* unmanaged<IntPtr, NetworkChangeKind, void> onNetworkChange
        );
    }
}
