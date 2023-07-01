using System;

namespace Microsoft.AspNetCore.Components
{
    internal static class PlatformInfo
    {
        public static bool IsWebAssembly { get; }

        static PlatformInfo()
        {
            IsWebAssembly = OperatingSystem.IsBrowser();
        }
    }
}
