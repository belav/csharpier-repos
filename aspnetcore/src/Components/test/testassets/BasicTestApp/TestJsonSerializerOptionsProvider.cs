using System.Text.Json;

namespace BasicTestApp;

internal static class TestJsonSerializerOptionsProvider
{
    public static JsonSerializerOptions Options { get; } =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
}
