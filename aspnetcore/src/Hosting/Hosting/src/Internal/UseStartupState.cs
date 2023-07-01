using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Hosting.Internal;

// Workaround for linker bug: https://github.com/dotnet/linker/issues/1981
internal readonly struct UseStartupState
{
    public UseStartupState(
        [DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType
    )
    {
        StartupType = startupType;
    }

    [DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)]
    public Type StartupType { get; }
}
