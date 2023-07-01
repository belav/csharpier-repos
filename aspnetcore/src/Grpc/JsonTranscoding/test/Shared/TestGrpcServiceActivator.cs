using Grpc.AspNetCore.Server;

namespace Grpc.Tests.Shared;

internal class TestGrpcServiceActivator<TGrpcService> : IGrpcServiceActivator<TGrpcService>
    where TGrpcService : class, new()
{
    public GrpcActivatorHandle<TGrpcService> Create(IServiceProvider serviceProvider)
    {
        return new GrpcActivatorHandle<TGrpcService>(new TGrpcService(), false, null);
    }

    public ValueTask ReleaseAsync(GrpcActivatorHandle<TGrpcService> service)
    {
        return default;
    }
}
