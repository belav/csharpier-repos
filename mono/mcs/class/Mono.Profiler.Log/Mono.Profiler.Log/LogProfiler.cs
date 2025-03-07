// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Mono.Profiler.Log
{
    public static class LogProfiler
    {
        static bool? _attached;

        public static bool IsAttached
        {
            get
            {
                if (_attached != null)
                    return (bool)_attached;

                try
                {
                    GetMaxStackTraceFrames();
                    return (bool)(_attached = true);
                }
                catch (Exception e) when (e is MissingMethodException || e is SecurityException)
                {
                    return (bool)(_attached = false);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern int GetMaxStackTraceFrames();

        public static int MaxStackTraceFrames
        {
            get { return GetMaxStackTraceFrames(); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern int GetStackTraceFrames();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetStackTraceFrames(int value);

        public static int StackTraceFrames
        {
            get { return GetStackTraceFrames(); }
            set
            {
                var max = MaxStackTraceFrames;

                if (value < 0 || value > max)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        $"Value must be between 0 and {max}."
                    );

                SetStackTraceFrames(value);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern LogHeapshotMode GetHeapshotMode();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetHeapshotMode(LogHeapshotMode value);

        public static LogHeapshotMode HeapshotMode
        {
            get { return GetHeapshotMode(); }
            set
            {
                if (!Enum.IsDefined(typeof(LogHeapshotMode), value))
                    throw new ArgumentException("Invalid heapshot mode.", nameof(value));

                SetHeapshotMode(value);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern int GetHeapshotMillisecondsFrequency();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetHeapshotMillisecondsFrequency(int value);

        public static int HeapshotMillisecondsFrequency
        {
            get { return GetHeapshotMillisecondsFrequency(); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Value must be non-negative."
                    );

                SetHeapshotMillisecondsFrequency(value);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern int GetHeapshotCollectionsFrequency();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetHeapshotCollectionsFrequency(int value);

        public static int HeapshotCollectionsFrequency
        {
            get { return GetHeapshotCollectionsFrequency(); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Value must be non-negative."
                    );

                SetHeapshotCollectionsFrequency(value);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void TriggerHeapshot();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern int GetCallDepth();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetCallDepth(int value);

        public static int CallDepth
        {
            get { return GetCallDepth(); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Value must be non-negative."
                    );

                SetCallDepth(value);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void GetSampleMode(out LogSampleMode mode, out int frequency);

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool SetSampleMode(LogSampleMode value, int frequency);

        public static LogSampleMode SampleMode
        {
            get
            {
                GetSampleMode(out var mode, out var _);

                return mode;
            }
        }

        public static int SampleFrequency
        {
            get
            {
                GetSampleMode(out var _, out var frequency);

                return frequency;
            }
        }

        public static bool SetSampleParameters(LogSampleMode mode, int frequency)
        {
            if (!Enum.IsDefined(typeof(LogSampleMode), mode))
                throw new ArgumentException("Invalid sample mode.", nameof(mode));

            if (frequency < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(frequency),
                    frequency,
                    "Frequency must be positive."
                );

            return SetSampleMode(mode, frequency);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetExceptionEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetExceptionEvents(bool value);

        public static bool ExceptionEventsEnabled
        {
            get { return GetExceptionEvents(); }
            set { SetExceptionEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetMonitorEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetMonitorEvents(bool value);

        public static bool MonitorEventsEnabled
        {
            get { return GetMonitorEvents(); }
            set { SetMonitorEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetGCEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetGCEvents(bool value);

        public static bool GCEventsEnabled
        {
            get { return GetGCEvents(); }
            set { SetGCEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetGCAllocationEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetGCAllocationEvents(bool value);

        public static bool GCAllocationEventsEnabled
        {
            get { return GetGCAllocationEvents(); }
            set { SetGCAllocationEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetGCMoveEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetGCMoveEvents(bool value);

        public static bool GCMoveEventsEnabled
        {
            get { return GetGCMoveEvents(); }
            set { SetGCMoveEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetGCRootEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetGCRootEvents(bool value);

        public static bool GCRootEventsEnabled
        {
            get { return GetGCRootEvents(); }
            set { SetGCRootEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetGCHandleEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetGCHandleEvents(bool value);

        public static bool GCHandleEventsEnabled
        {
            get { return GetGCHandleEvents(); }
            set { SetGCHandleEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetGCFinalizationEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetGCFinalizationEvents(bool value);

        public static bool GCFinalizationEventsEnabled
        {
            get { return GetGCFinalizationEvents(); }
            set { SetGCFinalizationEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetCounterEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetCounterEvents(bool value);

        public static bool CounterEventsEnabled
        {
            get { return GetCounterEvents(); }
            set { SetCounterEvents(value); }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern bool GetJitEvents();

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void SetJitEvents(bool value);

        public static bool JitEventsEnabled
        {
            get { return GetJitEvents(); }
            set { SetJitEvents(value); }
        }
    }
}
