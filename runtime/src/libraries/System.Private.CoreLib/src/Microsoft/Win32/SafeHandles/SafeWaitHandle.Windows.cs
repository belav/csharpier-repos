// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Win32.SafeHandles
{
    partial public sealed class SafeWaitHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected override bool ReleaseHandle() => Interop.Kernel32.CloseHandle(handle);
    }
}
