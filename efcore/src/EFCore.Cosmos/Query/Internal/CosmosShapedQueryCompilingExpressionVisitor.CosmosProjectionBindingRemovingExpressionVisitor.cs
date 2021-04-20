// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

#nullable disable

namespace Microsoft.EntityFrameworkCore.Cosmos.Query.Internal
{
    public partial class CosmosShapedQueryCompilingExpressionVisitor
    {
        private sealed class CosmosProjectionBindingRemovingExpressionVisitor : CosmosProjectionBindingRemovingExpressionVisitorBase
        {
            private readonly SelectExpression _selectExpression;

            public CosmosProjectionBindingRemovingExpressionVisitor(
                SelectExpression selectExpression,
                ParameterExpression jObjectParameter,
                bool trackQueryResults)
                : base(jObjectParameter, trackQueryResults)
            {
                _selectExpression = selectExpression;
            }

            protected override ProjectionExpression GetProjection(ProjectionBindingExpression projectionBindingExpression)
                => _selectExpression.Projection[GetProjectionIndex(projectionBindingExpression)];

            private int GetProjectionIndex(ProjectionBindingExpression projectionBindingExpression)
                => projectionBindingExpression.ProjectionMember != null
                    ? _selectExpression.GetMappedProjection(projectionBindingExpression.ProjectionMember).GetConstantValue<int>()
                    : projectionBindingExpression.Index
                    ?? throw new InvalidOperationException(CoreStrings.TranslationFailed(projectionBindingExpression.Print()));
        }
    }
}
