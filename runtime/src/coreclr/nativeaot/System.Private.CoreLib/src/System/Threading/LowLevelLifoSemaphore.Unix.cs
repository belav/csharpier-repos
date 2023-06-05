// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace System.Threading
{
    partial
    /// <summary>
    /// A LIFO semaphore.
    /// Waits on this semaphore are uninterruptible.
    /// </summary>
    internal sealed class LowLevelLifoSemaphore : IDisposable
    {
        private WaitSubsystem.WaitableObject _semaphore;

        private void Create(int maximumSignalCount)
        {
            _semaphore = WaitSubsystem.WaitableObject.NewSemaphore(0, maximumSignalCount);
        }

        public void Dispose() { }

        private bool WaitCore(int timeoutMs)
        {
            return WaitSubsystem.Wait(_semaphore, timeoutMs, false, true) == WaitHandle.WaitSuccess;
        }

        private void ReleaseCore(int count)
        {
            WaitSubsystem.ReleaseSemaphore(_semaphore, count);
        }
    }
}
