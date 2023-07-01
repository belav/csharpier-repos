using Grpc.AspNetCore.Server;
using Grpc.AspNetCore.Server.Model;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace Grpc.Shared.Server;

/// <summary>
/// Client streaming server method invoker.
/// </summary>
/// <typeparam name="TService">Service type for this method.</typeparam>
/// <typeparam name="TRequest">Request message type for this method.</typeparam>
/// <typeparam name="TResponse">Response message type for this method.</typeparam>
internal sealed class ClientStreamingServerMethodInvoker<TService, TRequest, TResponse>
    : ServerMethodInvokerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly ClientStreamingServerMethod<TService, TRequest, TResponse> _invoker;
    private readonly ClientStreamingServerMethod<TRequest, TResponse>? _pipelineInvoker;

    /// <summary>
    /// Creates a new instance of <see cref="ClientStreamingServerMethodInvoker{TService, TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="invoker">The client streaming method to invoke.</param>
    /// <param name="method">The description of the gRPC method.</param>
    /// <param name="options">The options used to execute the method.</param>
    /// <param name="serviceActivator">The service activator used to create service instances.</param>
    public ClientStreamingServerMethodInvoker(
        ClientStreamingServerMethod<TService, TRequest, TResponse> invoker,
        Method<TRequest, TResponse> method,
        MethodOptions options,
        IGrpcServiceActivator<TService> serviceActivator
    )
        : base(method, options, serviceActivator)
    {
        _invoker = invoker;

        if (Options.HasInterceptors)
        {
            var interceptorPipeline = new InterceptorPipelineBuilder<TRequest, TResponse>(
                Options.Interceptors
            );
            _pipelineInvoker = interceptorPipeline.ClientStreamingPipeline(
                ResolvedInterceptorInvoker
            );
        }
    }

    private async Task<TResponse> ResolvedInterceptorInvoker(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext resolvedContext
    )
    {
        GrpcActivatorHandle<TService> serviceHandle = default;
        try
        {
            serviceHandle = ServiceActivator.Create(
                resolvedContext.GetHttpContext().RequestServices
            );
            return await _invoker(serviceHandle.Instance, requestStream, resolvedContext);
        }
        finally
        {
            if (serviceHandle.Instance != null)
            {
                await ServiceActivator.ReleaseAsync(serviceHandle);
            }
        }
    }

    /// <summary>
    /// Invoke the client streaming method with the specified <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="serverCallContext">The <see cref="ServerCallContext"/>.</param>
    /// <param name="requestStream">The <typeparamref name="TRequest"/> reader.</param>
    /// <returns>A <see cref="Task{TResponse}"/> that represents the asynchronous method. The <see cref="Task{TResponse}.Result"/>
    /// property returns the <typeparamref name="TResponse"/> message.</returns>
    public async Task<TResponse> Invoke(
        HttpContext httpContext,
        ServerCallContext serverCallContext,
        IAsyncStreamReader<TRequest> requestStream
    )
    {
        if (_pipelineInvoker == null)
        {
            GrpcActivatorHandle<TService> serviceHandle = default;
            try
            {
                serviceHandle = ServiceActivator.Create(httpContext.RequestServices);
                return await _invoker(serviceHandle.Instance, requestStream, serverCallContext);
            }
            finally
            {
                if (serviceHandle.Instance != null)
                {
                    await ServiceActivator.ReleaseAsync(serviceHandle);
                }
            }
        }
        else
        {
            return await _pipelineInvoker(requestStream, serverCallContext);
        }
    }
}
