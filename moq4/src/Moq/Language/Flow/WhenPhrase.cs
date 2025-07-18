// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq.Language.Flow
{
    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class WhenPhrase<T> : ISetupConditionResult<T>
    After:
        sealed class WhenPhrase<T> : ISetupConditionResult<T>
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class WhenPhrase<T> : ISetupConditionResult<T>
    After:
        sealed class WhenPhrase<T> : ISetupConditionResult<T>
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class WhenPhrase<T> : ISetupConditionResult<T>
    After:
        sealed class WhenPhrase<T> : ISetupConditionResult<T>
    */
    sealed class WhenPhrase<T> : ISetupConditionResult<T>
        where T : class
    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private Mock<T> mock;
            private Condition condition;
    After:
            Mock<T> mock;
            Condition condition;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private Mock<T> mock;
            private Condition condition;
    After:
            Mock<T> mock;
            Condition condition;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private Mock<T> mock;
            private Condition condition;
    After:
            Mock<T> mock;
            Condition condition;
    */
    {
        Mock<T> mock;
        Condition condition;

        public WhenPhrase(Mock<T> mock, Condition condition)
        {
            this.mock = mock;
            this.condition = condition;
        }

        public ISetup<T> Setup(Expression<Action<T>> expression)
        {
            var setup = Mock.Setup(mock, expression, this.condition);
            return new VoidSetupPhrase<T>(setup);
        }

        public ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
        {
            var setup = Mock.Setup(mock, expression, this.condition);
            return new NonVoidSetupPhrase<T, TResult>(setup);
        }

        public ISetupGetter<T, TProperty> SetupGet<TProperty>(
            Expression<Func<T, TProperty>> expression
        )
        {
            var setup = Mock.SetupGet(mock, expression, this.condition);
            return new NonVoidSetupPhrase<T, TProperty>(setup);
        }

        public ISetupSetter<T, TProperty> SetupSet<TProperty>(Action<T> setterExpression)
        {
            Guard.NotNull(setterExpression, nameof(setterExpression));
            var expression = ExpressionReconstructor.Instance.ReconstructExpression(
                setterExpression,
                this.mock.ConstructorArguments
            );

            var setup = Mock.SetupSet(mock, expression, this.condition);
            return new SetterSetupPhrase<T, TProperty>(setup);
        }

        public ISetup<T> SetupSet(Action<T> setterExpression)
        {
            Guard.NotNull(setterExpression, nameof(setterExpression));
            var expression = ExpressionReconstructor.Instance.ReconstructExpression(
                setterExpression,
                this.mock.ConstructorArguments
            );

            var setup = Mock.SetupSet(mock, expression, this.condition);
            return new VoidSetupPhrase<T>(setup);
        }
    }
}
