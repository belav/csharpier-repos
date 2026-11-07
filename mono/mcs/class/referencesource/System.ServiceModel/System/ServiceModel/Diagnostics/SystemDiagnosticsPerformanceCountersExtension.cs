//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Diagnostics
{
    using System.Diagnostics;
    using System.Runtime;

    static class SystemDiagnosticsPerformanceCountersExtension
    {
        internal static void Increment(
            this PerformanceCountersBase thisPtr,
            PerformanceCounter[] counters,
            int counterIndex
        )
        {
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.Increment();
                    }
                }
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;
                PerformanceCounters.TracePerformanceCounterUpdateFailure(
                    thisPtr.InstanceName,
                    thisPtr.CounterNames[counterIndex]
                );
                if (counters != null)
                {
                    counters[counterIndex] = null;
                    PerformanceCounters.ReleasePerformanceCounter(ref counter);
                }
            }
        }

        internal static void IncrementBy(
            this PerformanceCountersBase thisPtr,
            PerformanceCounter[] counters,
            int counterIndex,
            long time
        )
        {
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.IncrementBy(time);
                    }
                }
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;
                PerformanceCounters.TracePerformanceCounterUpdateFailure(
                    thisPtr.InstanceName,
                    thisPtr.CounterNames[counterIndex]
                );
                if (counters != null)
                {
                    counters[counterIndex] = null;
                    PerformanceCounters.ReleasePerformanceCounter(ref counter);
                }
            }
        }

        internal static void Set(
            this PerformanceCountersBase thisPtr,
            PerformanceCounter[] counters,
            int counterIndex,
            long value
        )
        {
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.RawValue = value;
                    }
                }
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;
                PerformanceCounters.TracePerformanceCounterUpdateFailure(
                    thisPtr.InstanceName,
                    thisPtr.CounterNames[counterIndex]
                );
                counters[counterIndex] = null;
                PerformanceCounters.ReleasePerformanceCounter(ref counter);
            }
        }

        internal static void Decrement(
            this PerformanceCountersBase thisPtr,
            PerformanceCounter[] counters,
            int counterIndex
        )
        {
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.Decrement();
                    }
                }
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;
                PerformanceCounters.TracePerformanceCounterUpdateFailure(
                    thisPtr.InstanceName,
                    thisPtr.CounterNames[counterIndex]
                );
                if (counters != null)
                {
                    counters[counterIndex] = null;
                    PerformanceCounters.ReleasePerformanceCounter(ref counter);
                }
            }
        }
    }
}
