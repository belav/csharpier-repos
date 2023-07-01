using Google.Protobuf.Reflection;

namespace Microsoft.AspNetCore.Grpc.JsonTranscoding.Internal.Json;

internal sealed class JsonContext
{
    public JsonContext(
        GrpcJsonSettings settings,
        TypeRegistry typeRegistry,
        DescriptorRegistry descriptorRegistry
    )
    {
        Settings = settings;
        TypeRegistry = typeRegistry;
        DescriptorRegistry = descriptorRegistry;
    }

    public GrpcJsonSettings Settings { get; }
    public TypeRegistry TypeRegistry { get; }
    public DescriptorRegistry DescriptorRegistry { get; }
}
