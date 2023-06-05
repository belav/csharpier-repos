// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class JObjectLifetime
    {
        [LibraryImport(
            Libraries.AndroidCryptoNative,
            EntryPoint = "AndroidCryptoNative_NewGlobalReference"
        )]
        partial internal static IntPtr NewGlobalReference(IntPtr obj);

        [LibraryImport(
            Libraries.AndroidCryptoNative,
            EntryPoint = "AndroidCryptoNative_DeleteGlobalReference"
        )]
        partial internal static void DeleteGlobalReference(IntPtr obj);

        internal class SafeJObjectHandle : SafeHandle
        {
            public SafeJObjectHandle()
                : base(IntPtr.Zero, ownsHandle: true) { }

            internal SafeJObjectHandle(IntPtr ptr)
                : base(IntPtr.Zero, ownsHandle: true)
            {
                SetHandle(ptr);
            }

            protected SafeJObjectHandle(IntPtr ptr, bool ownsHandle)
                : base(IntPtr.Zero, ownsHandle)
            {
                SetHandle(ptr);
            }

            protected override bool ReleaseHandle()
            {
                Interop.JObjectLifetime.DeleteGlobalReference(handle);
                SetHandle(IntPtr.Zero);
                return true;
            }

            public override bool IsInvalid => handle == IntPtr.Zero;
        }
    }
}
