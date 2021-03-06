// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Threading;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Editor.UnitTests.Utilities
{
    public sealed class TestWaitContext : IWaitContext
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _maxUpdates;
        private int _updates;
        private readonly IProgressTracker _progressTracker;

        public TestWaitContext(int maxUpdates)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _maxUpdates = maxUpdates;
            _progressTracker = new ProgressTracker((_1, _2, _3) => UpdateProgress());
        }

        IProgressTracker IWaitContext.ProgressTracker => _progressTracker;

        public int Updates
        {
            get { return _updates; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        private void UpdateProgress()
        {
            var result = Interlocked.Increment(ref _updates);
            if (result > _maxUpdates)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public bool AllowCancel
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        public string Message
        {
            get
            {
                return "";
            }

            set
            {
            }
        }

        public void Dispose()
        {
        }
    }
}
