using Grpc.Core;
using Transcoding;

namespace Microsoft.AspNetCore.Grpc.JsonTranscoding.Tests.Infrastructure;

public class JsonTranscodingGreeterService : JsonTranscodingGreeter.JsonTranscodingGreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return base.SayHello(request, context);
    }
}
