// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Reflection.Internal;
using System.Reflection.PortableExecutable;
using Microsoft.Win32.SafeHandles;
using Xunit;

namespace System.Reflection.Metadata.Tests
{
    internal static unsafe class LoaderUtilities
    {
        public static void LoadPEAndValidate(
            byte[] peImage,
            Action<PEReader> validator,
            bool useStream = false
        )
        {
            using (var tempFile = new TempFile(Path.GetTempFileName()))
            {
                File.WriteAllBytes(tempFile.Path, peImage);

                using (
                    SafeLibraryHandle libHandle = global::Interop.Kernel32.LoadLibraryExW(
                        tempFile.Path,
                        IntPtr.Zero,
                        0
                    )
                )
                {
                    byte* peImagePtr = (byte*)
                        global::Interop.Kernel32.GetModuleHandle(Path.GetFileName(tempFile.Path));

                    Assert.True(peImagePtr != null);
                    Assert.Equal('M', (char)peImagePtr[0]);
                    Assert.Equal('Z', (char)peImagePtr[1]);

                    using (
                        var peReader = useStream
                            ? new PEReader(
                                new ReadOnlyUnmanagedMemoryStream(peImagePtr, int.MaxValue),
                                PEStreamOptions.IsLoadedImage
                            )
                            : new PEReader(peImagePtr, int.MaxValue, isLoadedImage: true)
                    )
                    {
                        validator(peReader);
                    }
                }
            }
        }
    }
}
