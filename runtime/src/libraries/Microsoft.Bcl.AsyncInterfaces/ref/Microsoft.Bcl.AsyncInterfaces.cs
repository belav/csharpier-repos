// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    partial public interface IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask DisposeAsync();
    }
}

namespace System.Collections.Generic
{
    partial public interface IAsyncEnumerable<out T>
    {
        System.Collections.Generic.IAsyncEnumerator<T> GetAsyncEnumerator(
            System.Threading.CancellationToken cancellationToken =
                default(System.Threading.CancellationToken)
        );
    }

    partial public interface IAsyncEnumerator<out T> : System.IAsyncDisposable
    {
        T Current { get; }
        System.Threading.Tasks.ValueTask<bool> MoveNextAsync();
    }
}

namespace System.Runtime.CompilerServices
{
    partial public struct AsyncIteratorMethodBuilder
    {
        private object _dummy;
        private int _dummyPrimitive;

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine
        )
            where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion
            where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine
        )
            where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion
            where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }

        public void Complete() { }

        public static System.Runtime.CompilerServices.AsyncIteratorMethodBuilder Create()
        {
            throw null;
        }

        public void MoveNext<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    }

    [System.AttributeUsageAttribute(
        System.AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = false
    )]
    partial public sealed class AsyncIteratorStateMachineAttribute
        : System.Runtime.CompilerServices.StateMachineAttribute
    {
        public AsyncIteratorStateMachineAttribute(System.Type stateMachineType)
            : base(default(System.Type)) { }
    }

    partial public readonly struct ConfiguredAsyncDisposable
    {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;

        public System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable DisposeAsync()
        {
            throw null;
        }
    }

    partial public readonly struct ConfiguredCancelableAsyncEnumerable<T>
    {
        private readonly object _dummy;
        private readonly int _dummyPrimitive;

        public System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable<T> ConfigureAwait(
            bool continueOnCapturedContext
        )
        {
            throw null;
        }

        public System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable<T>.Enumerator GetAsyncEnumerator()
        {
            throw null;
        }

        public System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable<T> WithCancellation(
            System.Threading.CancellationToken cancellationToken
        )
        {
            throw null;
        }

        partial public readonly struct Enumerator
        {
            private readonly object _dummy;
            private readonly int _dummyPrimitive;
            public T Current
            {
                get { throw null; }
            }

            public System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable DisposeAsync()
            {
                throw null;
            }

            public System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<bool> MoveNextAsync()
            {
                throw null;
            }
        }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Parameter, Inherited = false)]
    partial public sealed class EnumeratorCancellationAttribute : System.Attribute
    {
        public EnumeratorCancellationAttribute() { }
    }
}

namespace System.Threading.Tasks
{
    partial public static class TaskAsyncEnumerableExtensions
    {
        public static System.Runtime.CompilerServices.ConfiguredAsyncDisposable ConfigureAwait(
            this System.IAsyncDisposable source,
            bool continueOnCapturedContext
        )
        {
            throw null;
        }

        public static System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable<T> ConfigureAwait<T>(
            this System.Collections.Generic.IAsyncEnumerable<T> source,
            bool continueOnCapturedContext
        )
        {
            throw null;
        }

        public static System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable<T> WithCancellation<T>(
            this System.Collections.Generic.IAsyncEnumerable<T> source,
            System.Threading.CancellationToken cancellationToken
        )
        {
            throw null;
        }
    }
}

namespace System.Threading.Tasks.Sources
{
    partial public struct ManualResetValueTaskSourceCore<TResult>
    {
        private TResult _result;
        private object _dummy;
        private int _dummyPrimitive;
        public bool RunContinuationsAsynchronously
        {
            readonly get { throw null; }
            set { }
        }
        public short Version
        {
            get { throw null; }
        }

        public TResult GetResult(short token)
        {
            throw null;
        }

        public System.Threading.Tasks.Sources.ValueTaskSourceStatus GetStatus(short token)
        {
            throw null;
        }

        public void OnCompleted(
            System.Action<object?> continuation,
            object? state,
            short token,
            System.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags flags
        ) { }

        public void Reset() { }

        public void SetException(System.Exception error) { }

        public void SetResult(TResult result) { }
    }
}
