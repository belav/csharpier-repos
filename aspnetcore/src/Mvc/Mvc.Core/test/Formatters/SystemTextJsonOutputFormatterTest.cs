// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Mvc.Formatters;

public class SystemTextJsonOutputFormatterTest : JsonOutputFormatterTestBase
{
    protected override TextOutputFormatter GetOutputFormatter()
    {
        return SystemTextJsonOutputFormatter.CreateFormatter(new JsonOptions());
    }

    [Fact]
    public async Task WriteResponseBodyAsync_AllowsConfiguringPreserveReferenceHandling()
    {
        // Arrange
        var formatter = GetOutputFormatter();
        ((SystemTextJsonOutputFormatter)formatter).SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        var expectedContent = "{\"$id\":\"1\",\"name\":\"Person\",\"child\":{\"$id\":\"2\",\"name\":\"Child\",\"child\":null,\"parent\":{\"$ref\":\"1\"}},\"parent\":null}";
        var person = new Person
        {
            Name = "Person",
            Child = new Person { Name = "Child", },
        };
        person.Child.Parent = person;

        var mediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
        var encoding = CreateOrGetSupportedEncoding(formatter, "utf-8", isDefaultEncoding: true);

        var body = new MemoryStream();
        var actionContext = GetActionContext(mediaType, body);

        var outputFormatterContext = new OutputFormatterWriteContext(
            actionContext.HttpContext,
            new TestHttpResponseStreamWriterFactory().CreateWriter,
            typeof(Person),
            person)
        {
            ContentType = new StringSegment(mediaType.ToString()),
        };

        // Act
        await formatter.WriteResponseBodyAsync(outputFormatterContext, Encoding.GetEncoding("utf-8"));

        // Assert
        var actualContent = encoding.GetString(body.ToArray());
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task WriteResponseBodyAsync_WithNonUtf8Encoding_FormattingErrorsAreThrown()
    {
        // Arrange
        var formatter = GetOutputFormatter();
        var mediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-16");
        var encoding = CreateOrGetSupportedEncoding(formatter, "utf-16", isDefaultEncoding: true);

        var body = new MemoryStream();
        var actionContext = GetActionContext(mediaType, body);

        var outputFormatterContext = new OutputFormatterWriteContext(
            actionContext.HttpContext,
            new TestHttpResponseStreamWriterFactory().CreateWriter,
            typeof(Person),
            new ThrowingFormatterModel())
        {
            ContentType = new StringSegment(mediaType.ToString()),
        };

        // Act & Assert
        await Assert.ThrowsAsync<TimeZoneNotFoundException>(() => formatter.WriteResponseBodyAsync(outputFormatterContext, Encoding.GetEncoding("utf-16")));
    }

    [Fact]
    public async Task WriteResponseBodyAsync_ForLargeAsyncEnumerable()
    {
        // Arrange
        var expected = new MemoryStream();
        await JsonSerializer.SerializeAsync(expected, LargeAsync(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var formatter = GetOutputFormatter();
        var mediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
        var encoding = CreateOrGetSupportedEncoding(formatter, "utf-8", isDefaultEncoding: true);

        var body = new MemoryStream();
        var actionContext = GetActionContext(mediaType, body);

        var asyncEnumerable = LargeAsync();
        var outputFormatterContext = new OutputFormatterWriteContext(
            actionContext.HttpContext,
            new TestHttpResponseStreamWriterFactory().CreateWriter,
            asyncEnumerable.GetType(),
            asyncEnumerable)
        {
            ContentType = new StringSegment(mediaType.ToString()),
        };

        // Act
        await formatter.WriteResponseBodyAsync(outputFormatterContext, Encoding.GetEncoding("utf-8"));

        // Assert
        Assert.Equal(expected.ToArray(), body.ToArray());
    }

    [Fact]
    public async Task WriteResponseBodyAsync_AsyncEnumerableConnectionCloses()
    {
        // Arrange
        var formatter = GetOutputFormatter();
        var mediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        var body = new MemoryStream();
        var actionContext = GetActionContext(mediaType, body);
        var cts = new CancellationTokenSource();
        actionContext.HttpContext.RequestAborted = cts.Token;

        var asyncEnumerable = AsyncEnumerableClosedConnection();
        var outputFormatterContext = new OutputFormatterWriteContext(
            actionContext.HttpContext,
            new TestHttpResponseStreamWriterFactory().CreateWriter,
            asyncEnumerable.GetType(),
            asyncEnumerable)
        {
            ContentType = new StringSegment(mediaType.ToString()),
        };
        var iterated = false;

        // Act
        await formatter.WriteResponseBodyAsync(outputFormatterContext, Encoding.GetEncoding("utf-8"));

        // Assert
        // System.Text.Json might write the '[' before cancellation is observed
        Assert.InRange(body.ToArray().Length, 0, 1);
        Assert.False(iterated);

        async IAsyncEnumerable<int> AsyncEnumerableClosedConnection([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            cts.Cancel();
            // MvcOptions.MaxIAsyncEnumerableBufferLimit is 8192. Pick some value larger than that.
            foreach (var i in Enumerable.Range(0, 9000))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                iterated = true;
                yield return i;
            }
        }
    }

    private class Person
    {
        public string Name { get; set; }

        public Person Child { get; set; }

        public Person Parent { get; set; }
    }

    [JsonConverter(typeof(ThrowingFormatterPersonConverter))]
    private class ThrowingFormatterModel
    {

    }

    private class ThrowingFormatterPersonConverter : JsonConverter<ThrowingFormatterModel>
    {
        public override ThrowingFormatterModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ThrowingFormatterModel value, JsonSerializerOptions options)
        {
            throw new TimeZoneNotFoundException();
        }
    }

    private static async IAsyncEnumerable<int> LargeAsync()
    {
        await Task.Yield();
        // MvcOptions.MaxIAsyncEnumerableBufferLimit is 8192. Pick some value larger than that.
        foreach (var i in Enumerable.Range(0, 9000))
        {
            yield return i;
        }
    }
}
