// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Http.HttpResults;

public class CreatedOfTResultTests
{
    [Fact]
    public void CreatedResult_ProblemDetails_SetsStatusCodeAndValue()
    {
        // Arrange & Act
        var expectedUrl = "testAction";
        var obj = new HttpValidationProblemDetails();
        var result = new Created<HttpValidationProblemDetails>(expectedUrl, obj);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(StatusCodes.Status201Created, obj.Status);
        Assert.Equal(obj, result.Value);
    }

    [Fact]
    public void CreatedResult_SetsLocation()
    {
        // Arrange
        var location = "http://test/location";

        // Act
        var result = new Created<string>(location, "testInput");

        // Assert
        Assert.Same(location, result.Location);
    }

    [Fact]
    public async Task CreatedResult_ReturnsStatusCode_SetsLocationHeader()
    {
        // Arrange
        var location = "/test/";
        var httpContext = GetHttpContext();
        var result = new Created<string>(location, "testInput");

        // Act
        await result.ExecuteAsync(httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, httpContext.Response.StatusCode);
        Assert.Equal(location, httpContext.Response.Headers["Location"]);
    }

    [Fact]
    public async Task CreatedResult_OverwritesLocationHeader()
    {
        // Arrange
        var location = "/test/";
        var httpContext = GetHttpContext();
        httpContext.Response.Headers["Location"] = "/different/location/";
        var result = new Created<string>(location, "testInput");

        // Act
        await result.ExecuteAsync(httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, httpContext.Response.StatusCode);
        Assert.Equal(location, httpContext.Response.Headers["Location"]);
    }

    [Fact]
    public async Task CreatedResult_ExecuteResultAsync_FormatsData()
    {
        // Arrange
        var location = "/test/";
        var httpContext = GetHttpContext();
        var stream = new MemoryStream();
        httpContext.Response.Body = stream;
        httpContext.Response.Headers["Location"] = "/different/location/";
        var result = new Created<string>(location, "testInput");

        // Act
        await result.ExecuteAsync(httpContext);

        // Assert
        var response = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal("\"testInput\"", response);
    }

    [Fact]
    public void PopulateMetadata_AddsResponseTypeMetadata()
    {
        // Arrange
        Created<Todo> MyApi() { throw new NotImplementedException(); }
        var metadata = new List<object>();
        var context = new EndpointMetadataContext(((Delegate)MyApi).GetMethodInfo(), metadata, null);

        // Act
        PopulateMetadata<Created<Todo>>(context);

        // Assert
        var producesResponseTypeMetadata = context.EndpointMetadata.OfType<ProducesResponseTypeMetadata>().Last();
        Assert.Equal(StatusCodes.Status201Created, producesResponseTypeMetadata.StatusCode);
        Assert.Equal(typeof(Todo), producesResponseTypeMetadata.Type);
        Assert.Single(producesResponseTypeMetadata.ContentTypes, "application/json");
    }

    [Fact]
    public void ExecuteAsync_ThrowsArgumentNullException_WhenHttpContextIsNull()
    {
        // Arrange
        var result = new Created<object>("location", null);
        HttpContext httpContext = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>("httpContext", () => result.ExecuteAsync(httpContext));
    }

    [Fact]
    public void PopulateMetadata_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("context", () => PopulateMetadata<Created<object>>(null));
    }

    private static void PopulateMetadata<TResult>(EndpointMetadataContext context)
        where TResult : IEndpointMetadataProvider => TResult.PopulateMetadata(context);

    private record Todo(int Id, string Title);

    private static HttpContext GetHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.PathBase = new PathString("");
        httpContext.Response.Body = new MemoryStream();
        httpContext.RequestServices = CreateServices();
        return httpContext;
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();

        return services.BuildServiceProvider();
    }
}
