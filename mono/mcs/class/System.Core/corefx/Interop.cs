using System;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal static uint GetEUid()
        {
            throw new PlatformNotSupportedException();
        }

        internal static int SetEUid(uint euid)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
