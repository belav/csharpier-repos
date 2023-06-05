// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class WinMM
    {
        /// <summary>
        /// This function sends a data block to the specified waveform output device.
        /// </summary>
        /// <param name="hwo">Handle to the waveform-audio output device.</param>
        /// <param name="pwh">Pointer to a WaveHeader structure containing information
        /// about the data block.</param>
        /// <param name="cbwh">Size, in bytes, of the WaveHeader structure.</param>
        /// <returns>MMSYSERR</returns>
        [LibraryImport(Libraries.WinMM)]
        partial internal static MMSYSERR waveOutWrite(IntPtr hwo, IntPtr pwh, int cbwh);
    }
}
