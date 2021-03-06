// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.Http;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Http.Test
{
    public class HttpMessageHandlerBuilderTest
    {
        // Moq heavily utilizes RefEmit, which does not work on most aot workloads
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsReflectionEmitSupported))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/50873", TestPlatforms.Android)]
        public void Build_AdditionalHandlerIsNull_ThrowsException()
        {
            // Arrange
            var primaryHandler = Mock.Of<HttpMessageHandler>();
            var additionalHandlers = new DelegatingHandler[]
            {
                null,
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                HttpMessageHandlerBuilder.CreateHandlerPipeline(primaryHandler, additionalHandlers);
            });
            Assert.Equal("The 'additionalHandlers' must not contain a null entry.", exception.Message);
        }

        // Moq heavily utilizes RefEmit, which does not work on most aot workloads
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsReflectionEmitSupported))]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/50873", TestPlatforms.Android)]
        public void Build_AdditionalHandlerHasNonNullInnerHandler_ThrowsException()
        {
            // Arrange
            var primaryHandler = Mock.Of<HttpMessageHandler>();
            var additionalHandlers = new DelegatingHandler[]
            {
                Mock.Of<DelegatingHandler>(h => h.InnerHandler == Mock.Of<DelegatingHandler>()),
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                HttpMessageHandlerBuilder.CreateHandlerPipeline(primaryHandler, additionalHandlers);
            });
            Assert.Equal(
                "The 'InnerHandler' property must be null. " +
                "'DelegatingHandler' instances provided to 'HttpMessageHandlerBuilder' must not be reused or cached." + Environment.NewLine +
                $"Handler: '{additionalHandlers[0].ToString()}'",
                exception.Message);
        }
    }
}
