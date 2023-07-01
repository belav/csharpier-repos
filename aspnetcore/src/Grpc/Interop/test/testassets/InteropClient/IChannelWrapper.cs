using Grpc.Core;
using Grpc.Net.Client;

namespace InteropTestsClient;

public interface IChannelWrapper
{
    ChannelBase Channel { get; }
    Task ShutdownAsync();
}

public class GrpcChannelWrapper : IChannelWrapper
{
    public ChannelBase Channel { get; }

    public GrpcChannelWrapper(GrpcChannel channel)
    {
        Channel = channel;
    }

    public Task ShutdownAsync()
    {
        return Task.CompletedTask;
    }
}
