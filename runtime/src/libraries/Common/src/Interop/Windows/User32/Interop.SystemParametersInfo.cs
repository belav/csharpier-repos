// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class User32
    {
        public enum SystemParametersAction : uint
        {
            SPI_GETICONTITLELOGFONT = 0x1F,
            SPI_GETNONCLIENTMETRICS = 0x29
        }

        [LibraryImport(Libraries.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial public static unsafe bool SystemParametersInfoW(
            SystemParametersAction uiAction,
            uint uiParam,
            void* pvParam,
            uint fWinIni
        );
    }
}
