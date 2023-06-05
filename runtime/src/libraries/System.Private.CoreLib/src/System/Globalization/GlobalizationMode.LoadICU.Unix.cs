// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Globalization
{
    partial internal static class GlobalizationMode
    {
        private static int LoadICU() => Interop.Globalization.LoadICU();
    }
}
