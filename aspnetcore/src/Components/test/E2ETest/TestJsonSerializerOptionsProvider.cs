using System.Text.Json;

namespace Microsoft.AspNetCore.Components.E2ETest;

internal static class TestJsonSerializerOptionsProvider
{
    public static JsonSerializerOptions Options { get; } =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
}
