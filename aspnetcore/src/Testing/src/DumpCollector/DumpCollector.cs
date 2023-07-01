using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Testing;

public static partial class DumpCollector
{
    public static void Collect(Process process, string fileName)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Windows.Collect(process, fileName);
        }
        // No implementations yet for macOS and Linux
    }
}
