using System.Reflection;
using Microsoft.JSInterop.Infrastructure;

namespace Microsoft.JSInterop;

internal static class JSCallResultTypeHelper
{
    // We avoid using Assembly.GetExecutingAssembly() because this is shared code.
    private static readonly Assembly _currentAssembly = typeof(JSCallResultType).Assembly;

    public static JSCallResultType FromGeneric<TResult>()
    {
        if (typeof(TResult).Assembly == _currentAssembly)
        {
            if (
                typeof(TResult) == typeof(IJSObjectReference)
                || typeof(TResult) == typeof(IJSInProcessObjectReference)
                || typeof(TResult) == typeof(IJSUnmarshalledObjectReference)
            )
            {
                return JSCallResultType.JSObjectReference;
            }
            else if (typeof(TResult) == typeof(IJSStreamReference))
            {
                return JSCallResultType.JSStreamReference;
            }
            else if (typeof(TResult) == typeof(IJSVoidResult))
            {
                return JSCallResultType.JSVoidResult;
            }
        }

        return JSCallResultType.Default;
    }
}
