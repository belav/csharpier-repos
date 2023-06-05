// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetPosixSignalHandler")]
        [SuppressGCTransition]
        partial internal static unsafe void SetPosixSignalHandler(
            delegate* unmanaged<int, PosixSignal, int> handler
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_EnablePosixSignalHandling",
            SetLastError = true
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool EnablePosixSignalHandling(int signal);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_DisablePosixSignalHandling"
        )]
        partial internal static void DisablePosixSignalHandling(int signal);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_HandleNonCanceledPosixSignal"
        )]
        partial internal static void HandleNonCanceledPosixSignal(int signal);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetPlatformSignalNumber")]
        [SuppressGCTransition]
        partial internal static int GetPlatformSignalNumber(PosixSignal signal);
    }
}
