// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeLibraryHandle
**
** <EMAIL>Author: David Gutierrez (Microsoft) </EMAIL>
**
** A wrapper for a library handles
**
** Date:  July 8, 2002
**
===========================================================*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32.SafeHandles
{
    [HostProtectionAttribute(MayLeakOnAbort = true)]
    [SuppressUnmanagedCodeSecurityAttribute]
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Note that LoadLibraryEx returns 0 on failure

        internal SafeLibraryHandle()
            : base(true) { }

        [DllImport(
            ExternDll.Kernel32,
            CharSet = System.Runtime.InteropServices.CharSet.Unicode,
            SetLastError = true
        )]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern SafeLibraryHandle LoadLibraryEx(
            string libFilename,
            IntPtr reserved,
            int flags
        );

        [DllImport(ExternDll.Kernel32, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
#if !FEATURE_WINDOWSPHONE
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif // !FEATURE_WINDOWSPHONE
        private static extern bool FreeLibrary(IntPtr hModule);

        protected override bool ReleaseHandle()
        {
            return FreeLibrary(handle);
        }
    }
}
