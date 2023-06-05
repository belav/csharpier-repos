// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
    partial public static class NativeLibrary
    {
        internal static IntPtr LoadLibraryByName(
            string libraryName,
            Assembly assembly,
            DllImportSearchPath? searchPath,
            bool throwOnError
        )
        {
            RuntimeAssembly rtAsm = (RuntimeAssembly)assembly;
            return LoadByName(
                libraryName,
                new QCallAssembly(ref rtAsm),
                searchPath.HasValue,
                (uint)searchPath.GetValueOrDefault(),
                throwOnError
            );
        }

        /// External functions that implement the NativeLibrary interface

        [LibraryImport(
            RuntimeHelpers.QCall,
            EntryPoint = "NativeLibrary_LoadFromPath",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static IntPtr LoadFromPath(
            string libraryName,
            [MarshalAs(UnmanagedType.Bool)] bool throwOnError
        );

        [LibraryImport(
            RuntimeHelpers.QCall,
            EntryPoint = "NativeLibrary_LoadByName",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static IntPtr LoadByName(
            string libraryName,
            QCallAssembly callingAssembly,
            [MarshalAs(UnmanagedType.Bool)] bool hasDllImportSearchPathFlag,
            uint dllImportSearchPathFlag,
            [MarshalAs(UnmanagedType.Bool)] bool throwOnError
        );

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "NativeLibrary_FreeLib")]
        partial internal static void FreeLib(IntPtr handle);

        [LibraryImport(
            RuntimeHelpers.QCall,
            EntryPoint = "NativeLibrary_GetSymbol",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static IntPtr GetSymbol(
            IntPtr handle,
            string symbolName,
            [MarshalAs(UnmanagedType.Bool)] bool throwOnError
        );
    }
}
