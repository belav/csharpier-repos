// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    [DllImport("System.Native")]
    internal static extern void mono_pal_init();
}
