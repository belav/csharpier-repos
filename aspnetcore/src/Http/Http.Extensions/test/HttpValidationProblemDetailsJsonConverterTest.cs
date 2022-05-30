// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;

namespace Microsoft.AspNetCore.Http.Extensions;

public class HttpValidationProblemDetailsJsonConverterTest
{
    private static JsonSerializerOptions JsonSerializerOptions => new JsonOptions().SerializerOptions;

    [Fact]
    public void Read_Works()
    {
        // Arrange
        var type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        var title = "Not found";
        var status = 404;
        var detail = "Product not found";
        var instance = "http://example.com/products/14";
        var traceId = "|37dd3dd5-4a9619f953c40a16.";
        var json = $"{{\"type\":\"{type}\",\"title\":\"{title}\",\"status\":{status},\"detail\":\"{detail}\", \"instance\":\"{instance}\",\"traceId\":\"{traceId}\"," +
            "\"errors\":{\"key0\":[\"error0\"],\"key1\":[\"error1\",\"error2\"]}}";
        var converter = new HttpValidationProblemDetailsJsonConverter();
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        reader.Read();

        // Act
        var problemDetails = converter.Read(ref reader, typeof(HttpValidationProblemDetails), JsonSerializerOptions);

        Assert.Equal(type, problemDetails.Type);
        Assert.Equal(title, problemDetails.Title);
        Assert.Equal(status, problemDetails.Status);
        Assert.Equal(instance, problemDetails.Instance);
        Assert.Equal(detail, problemDetails.Detail);
        Assert.Collection(
            problemDetails.Extensions,
            kvp =>
            {
                Assert.Equal("traceId", kvp.Key);
                Assert.Equal(traceId, kvp.Value.ToString());
            });
        Assert.Collection(
            problemDetails.Errors.OrderBy(kvp => kvp.Key),
            kvp =>
            {
                Assert.Equal("key0", kvp.Key);
                Assert.Equal(new[] { "error0" }, kvp.Value);
            },
            kvp =>
            {
                Assert.Equal("key1", kvp.Key);
                Assert.Equal(new[] { "error1", "error2" }, kvp.Value);
            });
    }

    [Fact]
    public void Read_WithSomeMissingValues_Works()
    {
        // Arrange
        var type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        var title = "Not found";
        var status = 404;
        var traceId = "|37dd3dd5-4a9619f953c40a16.";
        var json = $"{{\"type\":\"{type}\",\"title\":\"{title}\",\"status\":{status},\"traceId\":\"{traceId}\"," +
            "\"errors\":{\"key0\":[\"error0\"],\"key1\":[\"error1\",\"error2\"]}}";
        var converter = new HttpValidationProblemDetailsJsonConverter();
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        reader.Read();

        // Act
        var problemDetails = converter.Read(ref reader, typeof(HttpValidationProblemDetails), JsonSerializerOptions);

        Assert.Equal(type, problemDetails.Type);
        Assert.Equal(title, problemDetails.Title);
        Assert.Equal(status, problemDetails.Status);
        Assert.Collection(
            problemDetails.Extensions,
            kvp =>
            {
                Assert.Equal("traceId", kvp.Key);
                Assert.Equal(traceId, kvp.Value.ToString());
            });
        Assert.Collection(
            problemDetails.Errors.OrderBy(kvp => kvp.Key),
            kvp =>
            {
                Assert.Equal("key0", kvp.Key);
                Assert.Equal(new[] { "error0" }, kvp.Value);
            },
            kvp =>
            {
                Assert.Equal("key1", kvp.Key);
                Assert.Equal(new[] { "error1", "error2" }, kvp.Value);
            });
    }

    [Fact]
    public void ReadUsingJsonSerializerWorks()
    {
        // Arrange
        var type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        var title = "Not found";
        var status = 404;
        var traceId = "|37dd3dd5-4a9619f953c40a16.";
        var json = $"{{\"type\":\"{type}\",\"title\":\"{title}\",\"status\":{status},\"traceId\":\"{traceId}\"," +
            "\"errors\":{\"key0\":[\"error0\"],\"key1\":[\"error1\",\"error2\"]}}";

        // Act
        var problemDetails = JsonSerializer.Deserialize<HttpValidationProblemDetails>(json, JsonSerializerOptions);

        Assert.Equal(type, problemDetails.Type);
        Assert.Equal(title, problemDetails.Title);
        Assert.Equal(status, problemDetails.Status);
        Assert.Collection(
            problemDetails.Extensions,
            kvp =>
            {
                Assert.Equal("traceId", kvp.Key);
                Assert.Equal(traceId, kvp.Value.ToString());
            });
        Assert.Collection(
            problemDetails.Errors.OrderBy(kvp => kvp.Key),
            kvp =>
            {
                Assert.Equal("key0", kvp.Key);
                Assert.Equal(new[] { "error0" }, kvp.Value);
            },
            kvp =>
            {
                Assert.Equal("key1", kvp.Key);
                Assert.Equal(new[] { "error1", "error2" }, kvp.Value);
            });
    }
}
