﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Microsoft.AspNetCore.Diagnostics.FunctionalTests;

public class StatusCodeSampleTest : IClassFixture<TestFixture<StatusCodePagesSample.Startup>>
{
    public StatusCodeSampleTest(TestFixture<StatusCodePagesSample.Startup> fixture)
    {
        Client = fixture.Client;
    }

    public HttpClient Client { get; }

    [Fact]
    public async Task StatusCodePage_ShowsError()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/errors/417");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Status Code: 417", body);
    }

    [Fact]
    public async Task StatusCodePageOptions_ExcludesSemicolon_WhenReasonPhrase_IsUnknown()
    {
        //Arrange
        var httpStatusCode = 541;
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost/?statuscode={httpStatusCode}");

        //Act
        var response = await Client.SendAsync(request);

        var statusCode = response.StatusCode;

        var responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal((HttpStatusCode)httpStatusCode, response.StatusCode);
        Assert.DoesNotContain(";", responseBody);
    }

    [Fact]
    public async Task StatusCodePageOptions_IncludesSemicolon__AndReasonPhrase_WhenReasonPhrase_IsKnown()
    {
        //Arrange
        var httpStatusCode = 400;
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost/?statuscode={httpStatusCode}");

        //Act
        var response = await Client.SendAsync(request);

        var statusCode = response.StatusCode;
        var statusCodeReasonPhrase = ReasonPhrases.GetReasonPhrase(httpStatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal((HttpStatusCode)httpStatusCode, response.StatusCode);
        Assert.Contains(";", responseBody);
        Assert.Contains(statusCodeReasonPhrase, responseBody);
    }
}
