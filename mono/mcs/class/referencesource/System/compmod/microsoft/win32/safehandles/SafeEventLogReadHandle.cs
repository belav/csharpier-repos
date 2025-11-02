// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeEventLogReadHandle
**
** <EMAIL>Author: David Gutierrez (Microsoft) </EMAIL>
**
** A wrapper for event log handles
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
    internal sealed class SafeEventLogReadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Note: OpenEventLog returns 0 on failure.

        internal SafeEventLogReadHandle()
            : base(true) { }

        [DllImport(
            ExternDll.Advapi32,
            CharSet = System.Runtime.InteropServices.CharSet.Unicode,
            SetLastError = true
        )]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern SafeEventLogReadHandle OpenEventLog(
            string UNCServerName,
            string sourceName
        );

        [DllImport(ExternDll.Advapi32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern bool CloseEventLog(IntPtr hEventLog);

        protected override bool ReleaseHandle()
        {
            return CloseEventLog(handle);
        }
    }
}
