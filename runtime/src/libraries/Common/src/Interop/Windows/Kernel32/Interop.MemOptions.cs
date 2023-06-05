partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Kernel32
    {
        partial internal static class MemOptions
        {
            internal const int MEM_COMMIT = 0x1000;
            internal const int MEM_RESERVE = 0x2000;
            internal const int MEM_RELEASE = 0x8000;
            internal const int MEM_FREE = 0x10000;
        }

        internal const int INVALID_FILE_SIZE = -1;

        partial internal static class PageOptions
        {
            internal const int PAGE_READWRITE = 0x04;
            internal const int PAGE_READONLY = 0x02;
            internal const int PAGE_WRITECOPY = 0x08;
            internal const int PAGE_EXECUTE_READ = 0x20;
            internal const int PAGE_EXECUTE_READWRITE = 0x40;
        }

        partial internal static class FileMapOptions
        {
            internal const int FILE_MAP_COPY = 0x0001;
            internal const int FILE_MAP_WRITE = 0x0002;
            internal const int FILE_MAP_READ = 0x0004;
            internal const int FILE_MAP_EXECUTE = 0x0020;
        }
    }
}
