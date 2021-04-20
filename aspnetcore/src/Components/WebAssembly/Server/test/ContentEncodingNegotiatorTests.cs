// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Components.WebAssembly.Server.Tests
{
    public class ContentEncodingNegotiatorTests
    {
        [Fact]
        public async Task RespectsAcceptEncodingQuality()
        {
            var encoding = "gzip;q=0.5, deflate;q=0.3, br;q=0.2";
            var expectedPath = "/_framework/blazor.boot.json.gz";
            var expectedEncoding = "gzip";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        [Fact]
        public async Task RespectsIdentity()
        {
            var encoding = "gzip;q=0.5, deflate;q=0.3, br;q=0.2, identity";
            var expectedPath = "/_framework/blazor.boot.json";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.False(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.False(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
        }

        [Fact]
        public async Task SkipsNonExistingFiles()
        {
            var encoding = "gzip;q=0.5, deflate;q=0.3, br";
            var expectedPath = "/_framework/blazor.boot.json.gz";
            var expectedEncoding = "gzip";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment(brotliExists: false));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        [Fact]
        public async Task UsesPreferredServerEncodingForEqualQualityValues()
        {
            var encoding = "gzip, deflate, br";
            var expectedPath = "/_framework/blazor.boot.json.br";
            var expectedEncoding = "br";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        [Fact]
        public async Task SkipNonExistingFilesWhenSearchingForServerPreferencesPreferences()
        {
            var encoding = "gzip, deflate, br";
            var expectedPath = "/_framework/blazor.boot.json.gz";
            var expectedEncoding = "gzip";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment(brotliExists: false));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        [Fact]
        public async Task AnyUsesServerPreference()
        {
            var encoding = "*";
            var expectedPath = "/_framework/blazor.boot.json.br";
            var expectedEncoding = "br";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        [Fact]
        public async Task AnySkipsNonExistingFiles()
        {
            var encoding = "*";
            var expectedPath = "/_framework/blazor.boot.json.gz";
            var expectedEncoding = "gzip";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment(brotliExists: false));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        [Fact]
        public async Task AnyDoesNotPickEncodingIfNoFilesFound()
        {
            var encoding = "*";
            var expectedPath = "/_framework/blazor.boot.json";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment(gzipExists: false, brotliExists: false));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.False(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.False(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
        }

        [Fact]
        public async Task AnyRespectsServerPreference()
        {
            var encoding = "gzip;q=0.5, *;q=0.8, br;q=0.2";
            var expectedPath = "/_framework/blazor.boot.json.br";
            var expectedEncoding = "br";
            RequestDelegate next = (ctx) => Task.CompletedTask;

            var negotiator = new ContentEncodingNegotiator(next, CreateWebHostEnvironment());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/_framework/blazor.boot.json";
            httpContext.Request.Headers.Append(HeaderNames.AcceptEncoding, encoding);

            await negotiator.InvokeAsync(httpContext);

            Assert.Equal(expectedPath, httpContext.Request.Path);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.ContentEncoding, out var selectedEncoding));
            Assert.Equal(expectedEncoding, selectedEncoding);
            Assert.True(httpContext.Response.Headers.TryGetValue(HeaderNames.Vary, out var varyHeader));
            Assert.Contains(HeaderNames.ContentEncoding, varyHeader.ToArray());
        }

        private static IWebHostEnvironment CreateWebHostEnvironment(bool gzipExists = true, bool brotliExists = true)
        {
            var gzMock = new Mock<IFileInfo>();
            gzMock.Setup(m => m.Exists).Returns(gzipExists);
            var brMock = new Mock<IFileInfo>();
            brMock.Setup(m => m.Exists).Returns(brotliExists);
            var fileProviderMock = new Mock<IFileProvider>();
            fileProviderMock.Setup(f => f.GetFileInfo("/_framework/blazor.boot.json.gz")).Returns(gzMock.Object);
            fileProviderMock.Setup(f => f.GetFileInfo("/_framework/blazor.boot.json.br")).Returns(brMock.Object);

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootFileProvider).Returns(fileProviderMock.Object);

            return env.Object;
        }
    }
}
