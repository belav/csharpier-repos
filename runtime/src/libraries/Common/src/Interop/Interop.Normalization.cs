// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Text;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_IsNormalized",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int IsNormalized(
            NormalizationForm normalizationForm,
            char* src,
            int srcLen
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_NormalizeString",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int NormalizeString(
            NormalizationForm normalizationForm,
            char* src,
            int srcLen,
            char* dstBuffer,
            int dstBufferCapacity
        );
    }
}
