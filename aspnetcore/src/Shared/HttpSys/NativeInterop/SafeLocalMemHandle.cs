using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.AspNetCore.HttpSys.Internal;

internal sealed class SafeLocalMemHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    internal SafeLocalMemHandle()
        : base(true) { }

    internal SafeLocalMemHandle(IntPtr existingHandle, bool ownsHandle)
        : base(ownsHandle)
    {
        SetHandle(existingHandle);
    }

    protected override bool ReleaseHandle()
    {
        return UnsafeNclNativeMethods.SafeNetHandles.LocalFree(handle) == IntPtr.Zero;
    }
}
