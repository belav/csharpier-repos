using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Type = System.Type;
using Google.Protobuf;

namespace Microsoft.AspNetCore.Grpc.JsonTranscoding.Internal.Json;

internal sealed class JsonConverterFactoryForMessage : JsonConverterFactory
{
    private readonly JsonContext _context;

    public JsonConverterFactoryForMessage(JsonContext context)
    {
        _context = context;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IMessage).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        JsonConverter converter = (JsonConverter)
            Activator.CreateInstance(
                typeof(MessageConverter<>).MakeGenericType(new Type[] { typeToConvert }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { _context },
                culture: null
            )!;

        return converter;
    }
}
