// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeThreadHandle
**
**
** A wrapper for a thread handle
**
**
===========================================================*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Win32.SafeHandles
{
    [SuppressUnmanagedCodeSecurityAttribute]
    internal sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeThreadHandle()
            : base(true) { }

        internal void InitialSetHandle(IntPtr h)
        {
            Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
            base.SetHandle(h);
        }

        protected override bool ReleaseHandle()
        {
            return SafeNativeMethods.CloseHandle(handle);
        }
    }
}
