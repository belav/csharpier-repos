// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeProcessHandle
**
** A wrapper for a process handle
**
**
===========================================================*/

using System;
using System.Diagnostics;
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
    [SuppressUnmanagedCodeSecurityAttribute]
    public sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal static SafeProcessHandle InvalidHandle = new SafeProcessHandle(IntPtr.Zero);

        // Note that OpenProcess returns 0 on failure

        internal SafeProcessHandle()
            : base(true) { }

        internal SafeProcessHandle(IntPtr handle)
            : base(true)
        {
            SetHandle(handle);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public SafeProcessHandle(IntPtr existingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

#if !MONO
        [DllImport(
            ExternDll.Kernel32,
            CharSet = System.Runtime.InteropServices.CharSet.Auto,
            SetLastError = true
        )]
        [ResourceExposure(ResourceScope.Machine)]
        internal static extern SafeProcessHandle OpenProcess(
            int access,
            bool inherit,
            int processId
        );
#endif

        internal void InitialSetHandle(IntPtr h)
        {
            Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
            base.handle = h;
        }

        protected override bool ReleaseHandle()
        {
#if !MONO
            return SafeNativeMethods.CloseHandle(handle);
#else
            return NativeMethods.CloseProcess(handle);
#endif
        }
    }
}
