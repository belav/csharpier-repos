partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Kernel32
    {
        partial internal static class FileTypes
        {
            internal const int FILE_TYPE_UNKNOWN = 0x0000;
            internal const int FILE_TYPE_DISK = 0x0001;
            internal const int FILE_TYPE_CHAR = 0x0002;
            internal const int FILE_TYPE_PIPE = 0x0003;
        }
    }
}
