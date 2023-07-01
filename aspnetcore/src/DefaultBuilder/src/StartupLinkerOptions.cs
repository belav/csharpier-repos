using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore;

internal static class StartupLinkerOptions
{
    // We're going to keep all public constructors and public methods on Startup classes
    public const DynamicallyAccessedMemberTypes Accessibility =
        DynamicallyAccessedMemberTypes.PublicConstructors
        | DynamicallyAccessedMemberTypes.PublicMethods;
}
