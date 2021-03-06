// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Shared.Extensions
{
    internal static partial class IEnumerableExtensions
    {
        public static Task<IEnumerable<S>> SelectManyAsync<T, S>(
            this IEnumerable<T> sequence,
            Func<T, CancellationToken, Task<IEnumerable<S>>> selector, CancellationToken cancellationToken)
        {
            var whenAllTask = Task.WhenAll(sequence.Select(e => selector(e, cancellationToken)));

            return whenAllTask.SafeContinueWith(allResultsTask =>
                allResultsTask.Result.Flatten(),
                cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
        }
    }
}
