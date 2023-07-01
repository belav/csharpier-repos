using System.Reflection;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Represents the information accessible via the route handler filter
/// API when the user is constructing a new route handler.
/// </summary>
public sealed class EndpointFilterFactoryContext
{
    /// <summary>
    /// The <see cref="MethodInfo"/> associated with the current route handler, <see cref="RequestDelegate"/> or MVC action.
    /// </summary>
    /// <remarks>
    /// In the future this could support more endpoint types.
    /// </remarks>
    public required MethodInfo MethodInfo { get; init; }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance used to access application services.
    /// </summary>
    public IServiceProvider ApplicationServices { get; init; } = EmptyServiceProvider.Instance;

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public static EmptyServiceProvider Instance { get; } = new EmptyServiceProvider();

        public object? GetService(Type serviceType) => null;
    }
}
