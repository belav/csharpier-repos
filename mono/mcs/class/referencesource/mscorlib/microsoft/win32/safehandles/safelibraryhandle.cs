// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class: SafeLibraryHandle
**
============================================================*/
namespace Microsoft.Win32
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Versioning;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;

    [System.Security.SecurityCritical] // auto-generated
    [HostProtectionAttribute(MayLeakOnAbort = true)]
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeLibraryHandle()
            : base(true) { }

        [System.Security.SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return UnsafeNativeMethods.FreeLibrary(handle);
        }
    }
}
