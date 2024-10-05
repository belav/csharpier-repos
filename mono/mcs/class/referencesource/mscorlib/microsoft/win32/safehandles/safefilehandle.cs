// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeFileHandle
**
**
** A wrapper for file handles
**
**
===========================================================*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Microsoft.Win32;
#if !MONO
using System.Security.Permissions;
#endif

namespace Microsoft.Win32.SafeHandles
{
    [System.Security.SecurityCritical] // auto-generated_required
    public sealed class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeFileHandle()
            : base(true) { }

        public SafeFileHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        [System.Security.SecurityCritical]
#if !MONO
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
#endif
        override protected bool ReleaseHandle()
        {
#if MONO
            System.IO.MonoIOError error;
            System.IO.MonoIO.Close(handle, out error);
            return error == System.IO.MonoIOError.ERROR_SUCCESS;
#else
            return Win32Native.CloseHandle(handle);
#endif
        }
    }
}
