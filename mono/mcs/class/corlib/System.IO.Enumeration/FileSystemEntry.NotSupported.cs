using System.Collections;

namespace System.IO.Enumeration
{
    ref partial public unsafe struct FileSystemEntry
    {
        public ReadOnlySpan<char> Directory
        {
            get => throw new PlatformNotSupportedException();
        }
        public ReadOnlySpan<char> RootDirectory
        {
            get => throw new PlatformNotSupportedException();
        }
        public ReadOnlySpan<char> OriginalRootDirectory
        {
            get => throw new PlatformNotSupportedException();
        }
        public ReadOnlySpan<char> FileName
        {
            get => throw new PlatformNotSupportedException();
        }
        public bool IsDirectory => throw new PlatformNotSupportedException();

        public FileSystemInfo ToFileSystemInfo() => throw new PlatformNotSupportedException();
    }
}
