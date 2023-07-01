using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Microsoft.AspNetCore.Analyzers;

internal sealed class MiddlewareItem
{
    public MiddlewareItem(IInvocationOperation operation)
    {
        Operation = operation;
    }

    public IInvocationOperation Operation { get; }

    public IMethodSymbol UseMethod => Operation.TargetMethod;
}
