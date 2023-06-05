// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Ole32
    {
        internal static unsafe int CoGetObjectContext(in Guid riid, out IntPtr ppv)
        {
            fixed (Guid* riidPtr = &riid)
            fixed (IntPtr* ppvPtr = &ppv)
            {
                return CoGetObjectContext(riidPtr, ppvPtr);
            }
        }

        [LibraryImport(Libraries.Ole32)]
        partial internal static unsafe int CoGetObjectContext(Guid* riid, IntPtr* ppv);
    }
}
