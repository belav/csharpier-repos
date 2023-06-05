partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Kernel32
    {
        partial internal static class HandleTypes
        {
            internal const int STD_INPUT_HANDLE = -10;
            internal const int STD_OUTPUT_HANDLE = -11;
            internal const int STD_ERROR_HANDLE = -12;
        }
    }
}
