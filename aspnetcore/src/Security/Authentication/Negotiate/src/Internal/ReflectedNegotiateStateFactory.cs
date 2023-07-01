using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Authentication.Negotiate;

internal sealed class ReflectedNegotiateStateFactory : INegotiateStateFactory
{
    [RequiresUnreferencedCode(
        "Negotiate authentication uses types that cannot be statically analyzed."
    )]
    public INegotiateState CreateInstance()
    {
        return new ReflectedNegotiateState();
    }
}
