// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Components.WebView
{
    public class WebViewManagerTests
    {
        [Fact]
        public async Task CanRenderRootComponentAsync()
        {
            // Arrange
            var services = RegisterTestServices().AddTestBlazorWebView().BuildServiceProvider();
            var fileProvider = new TestFileProvider();
            var webViewManager = new TestWebViewManager(services, fileProvider);
            await webViewManager.AddRootComponentAsync(typeof(MyComponent), "#app", ParameterView.Empty);

            // Act
            Assert.Empty(webViewManager.SentIpcMessages);
            webViewManager.ReceiveAttachPageMessage();

            // Assert
            Assert.Collection(webViewManager.SentIpcMessages,
                m => AssertHelpers.IsAttachToDocumentMessage(m, 0, "#app"),
                m => AssertHelpers.IsRenderBatch(m));
        }

        [Fact]
        public async Task CanRenderRootComponent_AfterThePageIsAttachedAsync()
        {
            // Arrange
            var services = RegisterTestServices().AddTestBlazorWebView().BuildServiceProvider();
            var fileProvider = new TestFileProvider();
            var webViewManager = new TestWebViewManager(services, fileProvider);

            Assert.Empty(webViewManager.SentIpcMessages);
            webViewManager.ReceiveAttachPageMessage();

            // Act
            Assert.Empty(webViewManager.SentIpcMessages);
            await webViewManager.AddRootComponentAsync(typeof(MyComponent), "#app", ParameterView.Empty);

            // Assert
            Assert.Collection(webViewManager.SentIpcMessages,
                m => AssertHelpers.IsAttachToDocumentMessage(m, 0, "#app"),
                m => AssertHelpers.IsRenderBatch(m));
        }

        [Fact]
        public async Task AttachingToNewPage_DisposesExistingScopeAsync()
        {
            // Arrange
            var services = RegisterTestServices().AddTestBlazorWebView().BuildServiceProvider();
            var fileProvider = new TestFileProvider();
            var webViewManager = new TestWebViewManager(services, fileProvider);
            await webViewManager.AddRootComponentAsync(typeof(MyComponent), "#app", ParameterView.Empty);
            var singleton = services.GetRequiredService<SingletonService>();

            // Act
            Assert.Empty(webViewManager.SentIpcMessages);
            webViewManager.ReceiveAttachPageMessage();
            webViewManager.ReceiveAttachPageMessage();

            // Assert
            Assert.Collection(webViewManager.SentIpcMessages,
                m => AssertHelpers.IsAttachToDocumentMessage(m, 0, "#app"),
                m => AssertHelpers.IsRenderBatch(m),
                m => AssertHelpers.IsAttachToDocumentMessage(m, 0, "#app"),
                m => AssertHelpers.IsRenderBatch(m));

            Assert.Equal(2, singleton.Services.Count);
            Assert.NotSame(singleton.Services[0], singleton.Services[1]);
        }

        [Fact]
        public async Task AddRootComponentsWithExistingSelector_Throws()
        {
            // Arrange
            const string arbitraryComponentSelector = "some_selector";
            var services = RegisterTestServices().AddTestBlazorWebView().BuildServiceProvider();
            var fileProvider = new TestFileProvider();
            var webViewManager = new TestWebViewManager(services, fileProvider);
            await webViewManager.AddRootComponentAsync(typeof(MyComponent), arbitraryComponentSelector, ParameterView.Empty);

            // Act & assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await webViewManager.AddRootComponentAsync(typeof(MyComponent), arbitraryComponentSelector, ParameterView.Empty));

            Assert.Equal($"There is already a root component with selector '{arbitraryComponentSelector}'.", ex.Message);
        }

        private static IServiceCollection RegisterTestServices()
        {
            return new ServiceCollection().AddSingleton<SingletonService>().AddScoped<ScopedService>();
        }

        private class MyComponent : IComponent
        {
            private RenderHandle _handle;

            public void Attach(RenderHandle renderHandle)
            {
                _handle = renderHandle;
            }

            [Inject] public ScopedService MyScopedService { get; set; }

            public Task SetParametersAsync(ParameterView parameters)
            {
                _handle.Render(builder =>
                {
                    builder.OpenElement(0, "p");
                    builder.AddContent(1, "Hello world!");
                    builder.CloseElement();
                });

                return Task.CompletedTask;
            }
        }

        private class SingletonService
        {
            public List<ScopedService> Services { get; } = new();

            public void Add(ScopedService service)
            {
                Services.Add(service);
            }
        }

        private class ScopedService : IDisposable
        {
            public ScopedService(SingletonService singleton)
            {
                singleton.Add(this);
            }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
