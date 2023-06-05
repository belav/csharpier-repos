// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO.Ports;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Termios
    {
        [Flags]
        internal enum Signals
        {
            None = 0,
            SignalDtr = 1 << 0,
            SignalDsr = 1 << 1,
            SignalRts = 1 << 2,
            SignalCts = 1 << 3,
            SignalDcd = 1 << 4,
            SignalRng = 1 << 5,
            Error = -1,
        }

        internal enum Queue
        {
            AllQueues = 0,
            ReceiveQueue = 1,
            SendQueue = 2,
        }

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosReset",
            SetLastError = true
        )]
        partial internal static int TermiosReset(
            SafeSerialDeviceHandle handle,
            int speed,
            int data,
            StopBits stop,
            Parity parity,
            Handshake flow
        );

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosGetSignal",
            SetLastError = true
        )]
        partial internal static int TermiosGetSignal(SafeSerialDeviceHandle handle, Signals signal);

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosSetSignal",
            SetLastError = true
        )]
        partial internal static int TermiosGetSignal(
            SafeSerialDeviceHandle handle,
            Signals signal,
            int set
        );

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosGetAllSignals"
        )]
        partial internal static Signals TermiosGetAllSignals(SafeSerialDeviceHandle handle);

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosSetSpeed",
            SetLastError = true
        )]
        partial internal static int TermiosSetSpeed(SafeSerialDeviceHandle handle, int speed);

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosGetSpeed",
            SetLastError = true
        )]
        partial internal static int TermiosGetSpeed(SafeSerialDeviceHandle handle);

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosAvailableBytes",
            SetLastError = true
        )]
        partial internal static int TermiosGetAvailableBytes(
            SafeSerialDeviceHandle handle,
            [MarshalAs(UnmanagedType.Bool)] bool fromReadBuffer
        );

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosDiscard",
            SetLastError = true
        )]
        partial internal static int TermiosDiscard(SafeSerialDeviceHandle handle, Queue input);

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosDrain",
            SetLastError = true
        )]
        partial internal static int TermiosDrain(SafeSerialDeviceHandle handle);

        [LibraryImport(
            Libraries.IOPortsNative,
            EntryPoint = "SystemIoPortsNative_TermiosSendBreak",
            SetLastError = true
        )]
        partial internal static int TermiosSendBreak(SafeSerialDeviceHandle handle, int duration);
    }
}
