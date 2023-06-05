// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Text;

partial internal static class Interop
{
    partial internal static class Normaliz
    {
        [LibraryImport(
            "Normaliz.dll",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe BOOL IsNormalizedString(
            NormalizationForm normForm,
            char* source,
            int length
        );

        [LibraryImport(
            "Normaliz.dll",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int NormalizeString(
            NormalizationForm normForm,
            char* source,
            int sourceLength,
            char* destination,
            int destinationLength
        );
    }
}
