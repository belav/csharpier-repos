// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

using Moq.Behaviors;

namespace Moq.Language.Flow
{
    // keeping the fluent API separate from `SequenceMethodCall` saves us from having to
    // define a generic variant `SequenceMethodCallReturn<TResult>`, which would be much more
    // work that having a generic fluent API counterpart `SequenceMethodCall<TResult>`.
    [EditorBrowsable(EditorBrowsableState.Never)]
    sealed class SetupSequencePhrase : ISetupSequentialAction

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private SequenceSetup setup;
    After:
            SequenceSetup setup;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private SequenceSetup setup;
    After:
            SequenceSetup setup;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private SequenceSetup setup;
    After:
            SequenceSetup setup;
    */
    {
        SequenceSetup setup;

        public SetupSequencePhrase(SequenceSetup setup)
        {
            this.setup = setup;
        }

        public ISetupSequentialAction Pass()
        {
            this.setup.AddBehavior(NoOp.Instance);
            return this;
        }

        public ISetupSequentialAction Throws<TException>()
            where TException : Exception, new()
            => this.Throws(new TException());

        public ISetupSequentialAction Throws(Exception exception)
        {
            this.setup.AddBehavior(new ThrowException(exception));
            return this;
        }

        public ISetupSequentialAction Throws<TException>(Func<TException> exceptionFunction) where TException : Exception
        {
            Guard.NotNull(exceptionFunction, nameof(exceptionFunction));

            this.setup.AddBehavior(new ThrowComputedException(_ => exceptionFunction()));
            return this;
        }

        public override string ToString()
        {
            return setup.Expression.ToStringFixed();

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                internal sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
            After:
                sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                internal sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
            After:
                sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                internal sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
            After:
                sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
            */
        }
    }

    sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private SequenceSetup setup;
    After:
            SequenceSetup setup;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private SequenceSetup setup;
    After:
            SequenceSetup setup;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private SequenceSetup setup;
    After:
            SequenceSetup setup;
    */
    {
        SequenceSetup setup;

        public SetupSequencePhrase(SequenceSetup setup)
        {
            this.setup = setup;
        }

        public ISetupSequentialResult<TResult> CallBase()
        {
            this.setup.AddBehavior(ReturnBase.Instance);
            return this;
        }

        public ISetupSequentialResult<TResult> Returns(TResult value)
        {
            this.setup.AddBehavior(new ReturnValue(value));
            return this;
        }

        public ISetupSequentialResult<TResult> Returns(Func<TResult> valueFunction)
        {
            Guard.NotNull(valueFunction, nameof(valueFunction));

            // If `valueFunction` is `TResult`, that is someone is setting up the return value of a method
            // that returns a `TResult`, then we have arrived here because C# picked the wrong overload:
            // We don't want to invoke the passed delegate to get a return value; the passed delegate
            // already is the return value.
            if (valueFunction is TResult)
            {
                return this.Returns((TResult)(object)valueFunction);
            }

            this.setup.AddBehavior(new ReturnComputedValue(_ => valueFunction()));
            return this;
        }

        public ISetupSequentialResult<TResult> Throws(Exception exception)
        {
            this.setup.AddBehavior(new ThrowException(exception));
            return this;
        }

        public ISetupSequentialResult<TResult> Throws<TException>()
            where TException : Exception, new()
            => this.Throws(new TException());

        public ISetupSequentialResult<TResult> Throws<TException>(Func<TException> exceptionFunction) where TException : Exception
        {
            Guard.NotNull(exceptionFunction, nameof(exceptionFunction));

            this.setup.AddBehavior(new ThrowComputedException(_ => exceptionFunction()));
            return this;
        }

        public override string ToString()
        {
            return setup.Expression.ToStringFixed();
        }
    }
}
