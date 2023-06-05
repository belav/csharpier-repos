// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System
{
    partial public static class Environment
    {
        public static long WorkingSet =>
            (long)(
                Interop.procfs.TryReadStatusFile(ProcessId, out Interop.procfs.ParsedStatus status)
                    ? status.VmRSS
                    : 0
            );
    }
}
