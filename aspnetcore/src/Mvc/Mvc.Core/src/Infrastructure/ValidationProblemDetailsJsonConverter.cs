using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Infrastructure;

internal sealed class ValidationProblemDetailsJsonConverter
    : JsonConverter<ValidationProblemDetails>
{
    public override ValidationProblemDetails Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var problemDetails = new ValidationProblemDetails();
        HttpValidationProblemDetailsJsonConverter.ReadProblemDetails(
            ref reader,
            options,
            problemDetails
        );
        return problemDetails;
    }

    public override void Write(
        Utf8JsonWriter writer,
        ValidationProblemDetails value,
        JsonSerializerOptions options
    )
    {
        HttpValidationProblemDetailsJsonConverter.WriteProblemDetails(writer, value, options);
    }
}
