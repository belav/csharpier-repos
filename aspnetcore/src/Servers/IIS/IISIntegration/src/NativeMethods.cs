using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Server.IISIntegration;

internal static partial class NativeMethods
{
    private const string KERNEL32 = "kernel32.dll";

    [LibraryImport(KERNEL32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(IntPtr handle);
}
