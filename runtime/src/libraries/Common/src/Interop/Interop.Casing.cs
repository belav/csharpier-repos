// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_ChangeCase",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe void ChangeCase(
            char* src,
            int srcLen,
            char* dstBuffer,
            int dstBufferCapacity,
            [MarshalAs(UnmanagedType.Bool)] bool bToUpper
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_ChangeCaseInvariant",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe void ChangeCaseInvariant(
            char* src,
            int srcLen,
            char* dstBuffer,
            int dstBufferCapacity,
            [MarshalAs(UnmanagedType.Bool)] bool bToUpper
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_ChangeCaseTurkish",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe void ChangeCaseTurkish(
            char* src,
            int srcLen,
            char* dstBuffer,
            int dstBufferCapacity,
            [MarshalAs(UnmanagedType.Bool)] bool bToUpper
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_InitOrdinalCasingPage",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe void InitOrdinalCasingPage(int pageNumber, char* pTarget);
    }
}
