// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests;

public class JsonResultWithNewtonsoftJsonTest
    : IClassFixture<MvcTestFixture<BasicWebSite.StartupWithNewtonsoftJson>>
{
    private IServiceCollection _serviceCollection;

    public JsonResultWithNewtonsoftJsonTest(
        MvcTestFixture<BasicWebSite.StartupWithNewtonsoftJson> fixture
    )
    {
        var factory =
            fixture.Factories.FirstOrDefault()
            ?? fixture.WithWebHostBuilder(b =>
                b.UseStartup<BasicWebSite.StartupWithNewtonsoftJson>()
            );
        factory = factory.WithWebHostBuilder(b =>
            b.ConfigureTestServices(serviceCollection => _serviceCollection = serviceCollection)
        );

        Client = factory.CreateDefaultClient();
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task JsonResult_UsesDefaultContentType()
    {
        // Arrange
        var url = "http://localhost/JsonResultWithNewtonsoftJson/Plain";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act
        var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        Assert.Equal("{\"message\":\"hello\"}", content);
    }

    // Using an Accept header can't force Json to not be Json. If your accept header doesn't jive with the
    // formatters/content-type configured on the result it will be ignored.
    [Theory]
    [InlineData("application/xml")]
    [InlineData("text/xml")]
    public async Task JsonResult_Conneg_Fails(string mediaType)
    {
        // Arrange
        var url = "http://localhost/JsonResultWithNewtonsoftJson/Plain";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("Accept", mediaType);

        // Act
        var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        Assert.Equal("{\"message\":\"hello\"}", content);
    }

    // If the object is null, it will get formatted as JSON. NOT as a 204/NoContent
    [Fact]
    public async Task JsonResult_Null()
    {
        // Arrange
        var url = "http://localhost/JsonResultWithNewtonsoftJson/Null";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act
        var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        Assert.Equal("null", content);
    }

    // If the object is a string, it will get formatted as JSON. NOT as text/plain.
    [Fact]
    public async Task JsonResult_String()
    {
        // Arrange
        var url = "http://localhost/JsonResultWithNewtonsoftJson/String";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act
        var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        Assert.Equal("\"hello\"", content);
    }

    [Fact]
    public async Task JsonResult_Uses_CustomSerializerSettings()
    {
        // Arrange
        var url = "http://localhost/JsonResultWithNewtonsoftJson/CustomSerializerSettings";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act
        var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"message\":\"hello\"}", content);
    }

    [Fact]
    public async Task JsonResult_CustomContentType()
    {
        // Arrange
        var url = "http://localhost/JsonResultWithNewtonsoftJson/CustomContentType";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act
        var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/message+json", response.Content.Headers.ContentType.MediaType);
        Assert.Equal("{\"message\":\"hello\"}", content);
    }
}
