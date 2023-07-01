using System.Globalization;
using System.Text.Json;
using Type = System.Type;

namespace Microsoft.AspNetCore.Grpc.JsonTranscoding.Internal.Json;

internal sealed class Int64Converter : SettingsConverterBase<long>
{
    public Int64Converter(JsonContext context)
        : base(context) { }

    public override long Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return long.Parse(reader.GetString()!, CultureInfo.InvariantCulture);
        }

        return reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        if (!Context.Settings.WriteInt64sAsStrings)
        {
            writer.WriteNumberValue(value);
        }
        else
        {
            writer.WriteStringValue(value.ToString("d", CultureInfo.InvariantCulture));
        }
    }
}
