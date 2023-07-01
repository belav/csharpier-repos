using System.Text.Json;

namespace Microsoft.AspNetCore.Components;

internal static class JsonSerializerOptionsProvider
{
    public static readonly JsonSerializerOptions Options =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
}
