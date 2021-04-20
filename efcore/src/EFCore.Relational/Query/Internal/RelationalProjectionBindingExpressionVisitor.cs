// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class RelationalProjectionBindingExpressionVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo _getParameterValueMethodInfo
            = typeof(RelationalProjectionBindingExpressionVisitor).GetRequiredDeclaredMethod(nameof(GetParameterValue));

        private readonly RelationalQueryableMethodTranslatingExpressionVisitor _queryableMethodTranslatingExpressionVisitor;
        private readonly RelationalSqlTranslatingExpressionVisitor _sqlTranslator;
        private readonly IncludeFindingExpressionVisitor _includeFindingExpressionVisitor;

        private SelectExpression _selectExpression;
        private SqlExpression[] _existingProjections;
        private bool _clientEval;
        private Dictionary<EntityProjectionExpression, ProjectionBindingExpression>? _entityProjectionCache;

        private readonly Dictionary<ProjectionMember, Expression> _projectionMapping = new();
        private readonly Stack<ProjectionMember> _projectionMembers = new();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public RelationalProjectionBindingExpressionVisitor(
            RelationalQueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor,
            RelationalSqlTranslatingExpressionVisitor sqlTranslatingExpressionVisitor)
        {
            _queryableMethodTranslatingExpressionVisitor = queryableMethodTranslatingExpressionVisitor;
            _sqlTranslator = sqlTranslatingExpressionVisitor;
            _includeFindingExpressionVisitor = new IncludeFindingExpressionVisitor();
            _selectExpression = null!;
            _existingProjections = Array.Empty<SqlExpression>();
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual Expression Translate(SelectExpression selectExpression, Expression expression)
        {
            _selectExpression = selectExpression;
            _clientEval = false;

            _projectionMembers.Push(new ProjectionMember());

            var expandedExpression = _queryableMethodTranslatingExpressionVisitor.ExpandWeakEntities(_selectExpression, expression);
            var result = Visit(expandedExpression);

            if (result == QueryCompilationContext.NotTranslatedExpression)
            {
                _clientEval = true;
                _entityProjectionCache = new();

                expandedExpression = _queryableMethodTranslatingExpressionVisitor.ExpandWeakEntities(_selectExpression, expression);
                _existingProjections = _selectExpression.Projection.Select(e => e.Expression).ToArray();
                _selectExpression.ClearProjection();
                result = Visit(expandedExpression);

                _projectionMapping.Clear();
            }

            _selectExpression.ReplaceProjectionMapping(_projectionMapping);
            _selectExpression = null!;
            _projectionMembers.Clear();
            _projectionMapping.Clear();

            result = MatchTypes(result!, expression.Type);

            return result;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [return: NotNullIfNotNull("expression")]
        public override Expression? Visit(Expression? expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (!(expression is NewExpression
                || expression is MemberInitExpression
                || expression is EntityShaperExpression
                || expression is IncludeExpression))
            {
                // This skips the group parameter from GroupJoin
                if (expression is ParameterExpression parameter
                    && parameter.Type.IsGenericType
                    && parameter.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return parameter;
                }

                if (_clientEval)
                {
                    switch (expression)
                    {
                        case ConstantExpression _:
                            return expression;

                        case ProjectionBindingExpression projectionBindingExpression:
                            if (projectionBindingExpression.Index is int index)
                            {
                                var newIndex = _selectExpression.AddToProjection(_existingProjections[index]);

                                return new ProjectionBindingExpression(_selectExpression, newIndex, expression.Type);
                            }

                            if (projectionBindingExpression.ProjectionMember != null)
                            {
                                // This would be SqlExpression. EntityProjectionExpression would be wrapped inside EntityShaperExpression.
                                var mappedProjection = (SqlExpression)((SelectExpression)projectionBindingExpression.QueryExpression)
                                    .GetMappedProjection(projectionBindingExpression.ProjectionMember);

                                return new ProjectionBindingExpression(
                                    _selectExpression, _selectExpression.AddToProjection(mappedProjection), expression.Type);
                            }

                            throw new InvalidOperationException(CoreStrings.TranslationFailed(projectionBindingExpression.Print()));

                        case ParameterExpression parameterExpression:
                            if (parameterExpression.Name?.StartsWith(QueryCompilationContext.QueryParameterPrefix, StringComparison.Ordinal)
                                == true)
                            {
                                return Expression.Call(
                                    _getParameterValueMethodInfo.MakeGenericMethod(parameterExpression.Type),
                                    QueryCompilationContext.QueryContextParameter,
                                    Expression.Constant(parameterExpression.Name));
                            }

                            throw new InvalidOperationException(CoreStrings.TranslationFailed(parameterExpression.Print()));

                        case MaterializeCollectionNavigationExpression materializeCollectionNavigationExpression:
                            return _selectExpression.AddCollectionProjection(
                                _queryableMethodTranslatingExpressionVisitor.TranslateSubquery(
                                    materializeCollectionNavigationExpression.Subquery)!,
                                materializeCollectionNavigationExpression.Navigation,
                                materializeCollectionNavigationExpression.Navigation.ClrType.GetSequenceType());

                        case MethodCallExpression methodCallExpression:
                        {
                            if (methodCallExpression.Method.IsGenericMethod
                                && methodCallExpression.Method.DeclaringType == typeof(Enumerable)
                                && methodCallExpression.Method.Name == nameof(Enumerable.ToList))
                            {
                                var elementType = methodCallExpression.Method.GetGenericArguments()[0];

                                var result = _queryableMethodTranslatingExpressionVisitor.TranslateSubquery(
                                    methodCallExpression.Arguments[0]);

                                if (result != null)
                                {
                                    return _selectExpression.AddCollectionProjection(result, null, elementType);
                                }
                            }
                            else
                            {
                                var subquery = _queryableMethodTranslatingExpressionVisitor.TranslateSubquery(methodCallExpression);
                                if (subquery != null)
                                {
                                    if (subquery.ResultCardinality == ResultCardinality.Enumerable)
                                    {
                                        return _selectExpression.AddCollectionProjection(subquery, null, subquery.ShaperExpression.Type);
                                    }

                                    static bool IsAggregateResultWithCustomShaper(MethodInfo method)
                                    {
                                        if (method.IsGenericMethod)
                                        {
                                            method = method.GetGenericMethodDefinition();
                                        }

                                        return QueryableMethods.IsAverageWithoutSelector(method)
                                            || QueryableMethods.IsAverageWithSelector(method)
                                            || method == QueryableMethods.MaxWithoutSelector
                                            || method == QueryableMethods.MaxWithSelector
                                            || method == QueryableMethods.MinWithoutSelector
                                            || method == QueryableMethods.MinWithSelector
                                            || QueryableMethods.IsSumWithoutSelector(method)
                                            || QueryableMethods.IsSumWithSelector(method);
                                    }

                                    if (!(subquery.ShaperExpression is ProjectionBindingExpression
                                        || (subquery.ShaperExpression is UnaryExpression unaryExpression
                                            && unaryExpression.NodeType == ExpressionType.Convert
                                            && unaryExpression.Type.MakeNullable() == unaryExpression.Operand.Type
                                            && unaryExpression.Operand is ProjectionBindingExpression)
                                        || IsAggregateResultWithCustomShaper(methodCallExpression.Method)))
                                    {
                                        return _selectExpression.AddSingleProjection(subquery);
                                    }
                                }
                            }

                            break;
                        }
                    }

                    var translation = _sqlTranslator.Translate(expression);
                    return translation == null
                        ? base.Visit(expression)
                        : new ProjectionBindingExpression(
                            _selectExpression, _selectExpression.AddToProjection(translation), expression.Type.MakeNullable());
                }
                else
                {
                    var translation = _sqlTranslator.Translate(expression);
                    if (translation == null)
                    {
                        return QueryCompilationContext.NotTranslatedExpression;
                    }

                    _projectionMapping[_projectionMembers.Peek()] = translation;

                    return new ProjectionBindingExpression(_selectExpression, _projectionMembers.Peek(), expression.Type.MakeNullable());
                }
            }

            return base.Visit(expression);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            var left = MatchTypes(Visit(binaryExpression.Left), binaryExpression.Left.Type);
            var right = MatchTypes(Visit(binaryExpression.Right), binaryExpression.Right.Type);

            return binaryExpression.Update(left, VisitAndConvert(binaryExpression.Conversion, "VisitBinary"), right);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitConditional(ConditionalExpression conditionalExpression)
        {
            var test = Visit(conditionalExpression.Test);
            var ifTrue = Visit(conditionalExpression.IfTrue);
            var ifFalse = Visit(conditionalExpression.IfFalse);

            if (test.Type == typeof(bool?))
            {
                test = Expression.Equal(test, Expression.Constant(true, typeof(bool?)));
            }

            ifTrue = MatchTypes(ifTrue, conditionalExpression.IfTrue.Type);
            ifFalse = MatchTypes(ifFalse, conditionalExpression.IfFalse.Type);

            return conditionalExpression.Update(test, ifTrue, ifFalse);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitExtension(Expression extensionExpression)
        {
            Check.NotNull(extensionExpression, nameof(extensionExpression));

            switch (extensionExpression)
            {
                case EntityShaperExpression entityShaperExpression:
                {
                    // TODO: Make this easier to understand some day.
                    EntityProjectionExpression entityProjectionExpression;
                    if (entityShaperExpression.ValueBufferExpression is ProjectionBindingExpression projectionBindingExpression)
                    {
                        // If projectionBinding is not mapped then SelectExpression has client projection
                        // Hence force client eval
                        if (projectionBindingExpression.ProjectionMember == null)
                        {
                            if (_clientEval)
                            {
                                var indexMap = new Dictionary<IProperty, int>();
                                foreach (var item in projectionBindingExpression.IndexMap!)
                                {
                                    indexMap[item.Key] = _selectExpression.AddToProjection(_existingProjections[item.Value]);
                                }

                                return entityShaperExpression.Update(new ProjectionBindingExpression(_selectExpression, indexMap));
                            }

                            return QueryCompilationContext.NotTranslatedExpression;
                        }

                        entityProjectionExpression = (EntityProjectionExpression)((SelectExpression)projectionBindingExpression.QueryExpression)
                            .GetMappedProjection(projectionBindingExpression.ProjectionMember);
                    }
                    else
                    {
                        entityProjectionExpression = (EntityProjectionExpression)entityShaperExpression.ValueBufferExpression;
                    }

                    if (_clientEval)
                    {
                        if (!_entityProjectionCache!.TryGetValue(entityProjectionExpression, out var entityProjectionBinding))
                        {
                            entityProjectionBinding = new ProjectionBindingExpression(
                                _selectExpression, _selectExpression.AddToProjection(entityProjectionExpression));
                            _entityProjectionCache[entityProjectionExpression] = entityProjectionBinding;
                        }

                        return entityShaperExpression.Update(entityProjectionBinding);
                    }

                    _projectionMapping[_projectionMembers.Peek()] = entityProjectionExpression;

                    return entityShaperExpression.Update(
                        new ProjectionBindingExpression(_selectExpression, _projectionMembers.Peek(), typeof(ValueBuffer)));
                }

                case IncludeExpression _:
                    return _clientEval ? base.VisitExtension(extensionExpression) : QueryCompilationContext.NotTranslatedExpression;

                case CollectionShaperExpression _:
                    return _clientEval ? extensionExpression : QueryCompilationContext.NotTranslatedExpression;

                default:
                    throw new InvalidOperationException(CoreStrings.TranslationFailed(extensionExpression.Print()));
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override ElementInit VisitElementInit(ElementInit elementInit)
            => elementInit.Update(elementInit.Arguments.Select(e => MatchTypes(Visit(e)!, e.Type)));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            var expression = Visit(memberExpression.Expression);
            Expression updatedMemberExpression = memberExpression.Update(
                expression != null ? MatchTypes(expression, memberExpression.Expression!.Type) : expression);

            if (expression?.Type.IsNullableType() == true
                && !_includeFindingExpressionVisitor.ContainsInclude(expression))
            {
                var nullableReturnType = memberExpression.Type.MakeNullable();
                if (!memberExpression.Type.IsNullableType())
                {
                    updatedMemberExpression = Expression.Convert(updatedMemberExpression, nullableReturnType);
                }

                updatedMemberExpression = Expression.Condition(
                    Expression.Equal(expression, Expression.Default(expression.Type)),
                    Expression.Constant(null, nullableReturnType),
                    updatedMemberExpression);
            }

            return updatedMemberExpression;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment memberAssignment)
        {
            var expression = memberAssignment.Expression;
            Expression? visitedExpression;
            if (_clientEval)
            {
                visitedExpression = Visit(memberAssignment.Expression);
            }
            else
            {
                var projectionMember = _projectionMembers.Peek().Append(memberAssignment.Member);
                _projectionMembers.Push(projectionMember);

                visitedExpression = Visit(memberAssignment.Expression);
                if (visitedExpression == QueryCompilationContext.NotTranslatedExpression)
                {
                    return memberAssignment.Update(Expression.Convert(visitedExpression, expression.Type));
                }

                _projectionMembers.Pop();
            }

            visitedExpression = MatchTypes(visitedExpression, expression.Type);

            return memberAssignment.Update(visitedExpression);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitMemberInit(MemberInitExpression memberInitExpression)
        {
            Check.NotNull(memberInitExpression, nameof(memberInitExpression));

            var newExpression = Visit(memberInitExpression.NewExpression);
            if (newExpression == QueryCompilationContext.NotTranslatedExpression)
            {
                return QueryCompilationContext.NotTranslatedExpression;
            }

            var newBindings = new MemberBinding[memberInitExpression.Bindings.Count];
            for (var i = 0; i < newBindings.Length; i++)
            {
                if (memberInitExpression.Bindings[i].BindingType != MemberBindingType.Assignment)
                {
                    return QueryCompilationContext.NotTranslatedExpression;
                }

                newBindings[i] = VisitMemberBinding(memberInitExpression.Bindings[i]);
                if (newBindings[i] is MemberAssignment memberAssignment
                    && memberAssignment.Expression is UnaryExpression unaryExpression
                    && unaryExpression.NodeType == ExpressionType.Convert
                    && unaryExpression.Operand == QueryCompilationContext.NotTranslatedExpression)
                {
                    return QueryCompilationContext.NotTranslatedExpression;
                }
            }

            return memberInitExpression.Update((NewExpression)newExpression, newBindings);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            var @object = Visit(methodCallExpression.Object);
            var arguments = new Expression[methodCallExpression.Arguments.Count];
            for (var i = 0; i < methodCallExpression.Arguments.Count; i++)
            {
                var argument = methodCallExpression.Arguments[i];
                arguments[i] = MatchTypes(Visit(argument)!, argument.Type);
            }

            Expression updatedMethodCallExpression = methodCallExpression.Update(
                @object != null ? MatchTypes(@object, methodCallExpression.Object!.Type) : @object!,
                arguments);

            if (@object?.Type.IsNullableType() == true
                && methodCallExpression.Object != null
                && !methodCallExpression.Object.Type.IsNullableType())
            {
                var nullableReturnType = methodCallExpression.Type.MakeNullable();
                if (!methodCallExpression.Type.IsNullableType())
                {
                    updatedMethodCallExpression = Expression.Convert(updatedMethodCallExpression, nullableReturnType);
                }

                return Expression.Condition(
                    Expression.Equal(@object, Expression.Default(@object.Type)),
                    Expression.Constant(null, nullableReturnType),
                    updatedMethodCallExpression);
            }

            return updatedMethodCallExpression;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitNew(NewExpression newExpression)
        {
            Check.NotNull(newExpression, nameof(newExpression));

            if (newExpression.Arguments.Count == 0)
            {
                return newExpression;
            }

            if (!_clientEval
                && newExpression.Members == null)
            {
                return QueryCompilationContext.NotTranslatedExpression;
            }

            var newArguments = new Expression[newExpression.Arguments.Count];
            for (var i = 0; i < newArguments.Length; i++)
            {
                var argument = newExpression.Arguments[i];
                Expression? visitedArgument;
                if (_clientEval)
                {
                    visitedArgument = Visit(argument);
                }
                else
                {
                    var projectionMember = _projectionMembers.Peek().Append(newExpression.Members![i]);
                    _projectionMembers.Push(projectionMember);
                    visitedArgument = Visit(argument);
                    if (visitedArgument == QueryCompilationContext.NotTranslatedExpression)
                    {
                        return QueryCompilationContext.NotTranslatedExpression;
                    }

                    _projectionMembers.Pop();
                }

                newArguments[i] = MatchTypes(visitedArgument!, argument.Type);
            }

            return newExpression.Update(newArguments);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitNewArray(NewArrayExpression newArrayExpression)
            => newArrayExpression.Update(newArrayExpression.Expressions.Select(e => MatchTypes(Visit(e)!, e.Type)));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            var operand = Visit(unaryExpression.Operand)!;

            return (unaryExpression.NodeType == ExpressionType.Convert
                    || unaryExpression.NodeType == ExpressionType.ConvertChecked)
                && unaryExpression.Type == operand.Type
                    ? operand
                    : unaryExpression.Update(MatchTypes(operand, unaryExpression.Operand.Type));
        }

        [DebuggerStepThrough]
        private static Expression MatchTypes(Expression expression, Type targetType)
        {
            if (targetType != expression.Type
                && targetType.TryGetElementType(typeof(IQueryable<>)) == null)
            {
                Check.DebugAssert(targetType.MakeNullable() == expression.Type, "expression.Type must be nullable of targetType");

                expression = Expression.Convert(expression, targetType);
            }

            return expression;
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static T GetParameterValue<T>(QueryContext queryContext, string parameterName)
#pragma warning restore IDE0052 // Remove unread private members
            => (T)queryContext.ParameterValues[parameterName]!;

        private sealed class IncludeFindingExpressionVisitor : ExpressionVisitor
        {
            private bool _containsInclude;

            public bool ContainsInclude(Expression expression)
            {
                _containsInclude = false;

                Visit(expression);

                return _containsInclude;
            }

            [return: NotNullIfNotNull("expression")]
            public override Expression? Visit(Expression? expression)
                => _containsInclude ? expression : base.Visit(expression);

            protected override Expression VisitExtension(Expression extensionExpression)
            {
                if (extensionExpression is IncludeExpression)
                {
                    _containsInclude = true;

                    return extensionExpression;
                }

                return base.VisitExtension(extensionExpression);
            }
        }
    }
}
