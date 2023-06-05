// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class NetSecurityNative
    {
        [LibraryImport(
            Interop.Libraries.NetSecurityNative,
            EntryPoint = "NetSecurityNative_IsNtlmInstalled"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool IsNtlmInstalled();

        [LibraryImport(
            Interop.Libraries.NetSecurityNative,
            EntryPoint = "NetSecurityNative_EnsureGssInitialized"
        )]
        partial private static int EnsureGssInitialized();

        static NetSecurityNative()
        {
            GssInitializer.Initialize();
        }

        internal static class GssInitializer
        {
            static GssInitializer()
            {
                if (EnsureGssInitialized() != 0)
                {
                    throw new InvalidOperationException();
                }
            }

            internal static void Initialize()
            {
                // No-op that exists to provide a hook for other static constructors.
            }
        }
    }
}
