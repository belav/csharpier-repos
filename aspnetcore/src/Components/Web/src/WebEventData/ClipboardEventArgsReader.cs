using System.Text.Json;

namespace Microsoft.AspNetCore.Components.Web;

internal static class ClipboardEventArgsReader
{
    private static readonly JsonEncodedText TypeKey = JsonEncodedText.Encode("type");

    internal static ClipboardEventArgs Read(JsonElement jsonElement)
    {
        var eventArgs = new ClipboardEventArgs();
        foreach (var property in jsonElement.EnumerateObject())
        {
            if (property.NameEquals(TypeKey.EncodedUtf8Bytes))
            {
                eventArgs.Type = property.Value.GetString()!;
            }
            else
            {
                throw new JsonException($"Unknown property {property.Name}");
            }
        }

        return eventArgs;
    }
}
