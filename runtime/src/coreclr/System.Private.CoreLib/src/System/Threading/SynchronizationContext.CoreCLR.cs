// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Threading
{
    partial public class SynchronizationContext
    {
        private static int InvokeWaitMethodHelper(
            SynchronizationContext syncContext,
            IntPtr[] waitHandles,
            bool waitAll,
            int millisecondsTimeout
        )
        {
            return syncContext.Wait(waitHandles, waitAll, millisecondsTimeout);
        }
    }
}
