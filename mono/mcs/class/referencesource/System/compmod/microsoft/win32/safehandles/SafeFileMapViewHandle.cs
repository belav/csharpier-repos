// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeFileMapViewHandle
**
** <EMAIL>Author: Brian Grunkemeyer (Microsoft) </EMAIL>
**
** A wrapper for handles returned from MapViewOfFile, used
** for shared memory.
**
** Date:  August 7, 2002
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
    internal sealed class SafeFileMapViewHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Note that MapViewOfFile returns 0 on failure

        internal SafeFileMapViewHandle()
            : base(true) { }

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern SafeFileMapViewHandle MapViewOfFile(
            SafeFileMappingHandle hFileMappingObject,
            int dwDesiredAccess,
            int dwFileOffsetHigh,
            int dwFileOffsetLow,
            UIntPtr dwNumberOfBytesToMap
        );

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern bool UnmapViewOfFile(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return UnmapViewOfFile(handle);
        }
    }
}
