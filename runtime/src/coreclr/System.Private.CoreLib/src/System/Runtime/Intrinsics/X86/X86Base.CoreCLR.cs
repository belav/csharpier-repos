// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Intrinsics.X86
{
    partial public abstract class X86Base
    {
        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "X86BaseCpuId")]
        partial private static unsafe void __cpuidex(
            int* cpuInfo,
            int functionId,
            int subFunctionId
        );
    }
}
