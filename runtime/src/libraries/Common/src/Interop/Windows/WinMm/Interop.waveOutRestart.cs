// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class WinMM
    {
        /// <summary>
        /// This function restarts a paused waveform output device.
        /// </summary>
        /// <param name="hwo">Handle to the waveform-audio output device.</param>
        /// <returns>MMSYSERR</returns>
        [LibraryImport(Libraries.WinMM)]
        partial internal static MMSYSERR waveOutRestart(IntPtr hwo);
    }
}
