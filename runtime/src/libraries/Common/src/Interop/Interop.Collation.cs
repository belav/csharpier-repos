// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetSortHandle",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static unsafe ResultCode GetSortHandle(
            string localeName,
            out IntPtr sortHandle
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_CloseSortHandle"
        )]
        partial internal static void CloseSortHandle(IntPtr handle);

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_CompareString",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int CompareString(
            IntPtr sortHandle,
            char* lpStr1,
            int cwStr1Len,
            char* lpStr2,
            int cwStr2Len,
            CompareOptions options
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_IndexOf",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int IndexOf(
            IntPtr sortHandle,
            char* target,
            int cwTargetLength,
            char* pSource,
            int cwSourceLength,
            CompareOptions options,
            int* matchLengthPtr
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_LastIndexOf",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int LastIndexOf(
            IntPtr sortHandle,
            char* target,
            int cwTargetLength,
            char* pSource,
            int cwSourceLength,
            CompareOptions options,
            int* matchLengthPtr
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_StartsWith",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [MethodImpl(MethodImplOptions.NoInlining)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool StartsWith(
            IntPtr sortHandle,
            char* target,
            int cwTargetLength,
            char* source,
            int cwSourceLength,
            CompareOptions options,
            int* matchedLength
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_EndsWith",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [MethodImpl(MethodImplOptions.NoInlining)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool EndsWith(
            IntPtr sortHandle,
            char* target,
            int cwTargetLength,
            char* source,
            int cwSourceLength,
            CompareOptions options,
            int* matchedLength
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_StartsWith",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool StartsWith(
            IntPtr sortHandle,
            string target,
            int cwTargetLength,
            string source,
            int cwSourceLength,
            CompareOptions options
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_EndsWith",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool EndsWith(
            IntPtr sortHandle,
            string target,
            int cwTargetLength,
            string source,
            int cwSourceLength,
            CompareOptions options
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetSortKey",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int GetSortKey(
            IntPtr sortHandle,
            char* str,
            int strLength,
            byte* sortKey,
            int sortKeyLength,
            CompareOptions options
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetSortVersion"
        )]
        partial internal static int GetSortVersion(IntPtr sortHandle);
    }
}
