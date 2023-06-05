// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    partial public abstract class FileSystemTest
    {
        [LibraryImport("libc", SetLastError = true)]
        partial protected static int geteuid();

        [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        partial protected static int mkfifo(string path, int mode);
    }
}
