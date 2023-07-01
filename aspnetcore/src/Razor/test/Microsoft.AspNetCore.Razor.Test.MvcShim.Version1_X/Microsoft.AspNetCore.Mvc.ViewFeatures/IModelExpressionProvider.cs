using System;
using System.Linq.Expressions;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

public interface IModelExpressionProvider
{
    ModelExpression CreateModelExpression<TModel, TValue>(
        ViewDataDictionary<TModel> viewData,
        Expression<Func<TModel, TValue>> expression
    );
}
