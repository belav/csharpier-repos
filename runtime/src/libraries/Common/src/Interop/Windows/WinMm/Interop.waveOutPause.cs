// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class WinMM
    {
        /// <summary>
        /// This function pauses playback on a specified waveform output device. The
        /// current playback position is saved. Use waveOutRestart to resume playback
        /// from the current playback position.
        /// </summary>
        /// <param name="hwo">Handle to the waveform-audio output device.</param>
        /// <returns>MMSYSERR</returns>
        [LibraryImport(Libraries.WinMM)]
        partial internal static MMSYSERR waveOutPause(IntPtr hwo);
    }
}
