using Grpc.AspNetCore.Server;
using Grpc.Core;

namespace Grpc.Shared.Server;

/// <summary>
/// Server method invoker base type.
/// </summary>
/// <typeparam name="TService">Service type for this method.</typeparam>
/// <typeparam name="TRequest">Request message type for this method.</typeparam>
/// <typeparam name="TResponse">Response message type for this method.</typeparam>
internal abstract class ServerMethodInvokerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    /// <summary>
    /// Gets the description of the gRPC method.
    /// </summary>
    public Method<TRequest, TResponse> Method { get; }

    /// <summary>
    /// Gets the options used to execute the method.
    /// </summary>
    public MethodOptions Options { get; }

    /// <summary>
    /// Gets the service activator used to create service instances.
    /// </summary>
    public IGrpcServiceActivator<TService> ServiceActivator { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ServerMethodInvokerBase{TService, TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="method">The description of the gRPC method.</param>
    /// <param name="options">The options used to execute the method.</param>
    /// <param name="serviceActivator">The service activator used to create service instances.</param>
    private protected ServerMethodInvokerBase(
        Method<TRequest, TResponse> method,
        MethodOptions options,
        IGrpcServiceActivator<TService> serviceActivator
    )
    {
        Method = method;
        Options = options;
        ServiceActivator = serviceActivator;
    }
}
