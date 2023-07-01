using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

internal static class TestJsonSerializerOptionsProvider
{
    public static JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
