// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Test.Helpers;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Components.Virtualization
{
    public class VirtualizeTest
    {
        [Fact]
        public async Task Virtualize_ThrowsWhenGivenNonPositiveItemSize()
        {
            var rootComponent = new VirtualizeTestHostcomponent
            {
                InnerContent = BuildVirtualize(0f, EmptyItemsProvider<int>, null)
            };

            var serviceProvider = new ServiceCollection()
                .AddTransient((sp) => Mock.Of<IJSRuntime>())
                .BuildServiceProvider();

            var testRenderer = new TestRenderer(serviceProvider);
            var componentId = testRenderer.AssignRootComponentId(rootComponent);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await testRenderer.RenderRootComponentAsync(componentId));
            Assert.Contains("requires a positive value for parameter", ex.Message);
        }

        [Fact]
        public async Task Virtualize_ThrowsWhenGivenMultipleItemSources()
        {
            var rootComponent = new VirtualizeTestHostcomponent
            {
                InnerContent = BuildVirtualize(10f, EmptyItemsProvider<int>, new List<int>())
            };

            var serviceProvider = new ServiceCollection()
                .AddTransient((sp) => Mock.Of<IJSRuntime>())
                .BuildServiceProvider();

            var testRenderer = new TestRenderer(serviceProvider);
            var componentId = testRenderer.AssignRootComponentId(rootComponent);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await testRenderer.RenderRootComponentAsync(componentId));
            Assert.Contains("can only accept one item source from its parameters", ex.Message);
        }

        [Fact]
        public async Task Virtualize_ThrowsWhenGivenNoItemSources()
        {
            var rootComponent = new VirtualizeTestHostcomponent
            {
                InnerContent = BuildVirtualize<int>(10f, null, null)
            };

            var serviceProvider = new ServiceCollection()
                .AddTransient((sp) => Mock.Of<IJSRuntime>())
                .BuildServiceProvider();

            var testRenderer = new TestRenderer(serviceProvider);
            var componentId = testRenderer.AssignRootComponentId(rootComponent);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await testRenderer.RenderRootComponentAsync(componentId));
            Assert.Contains("parameters to be specified and non-null", ex.Message);
        }

        [Fact]
        public async Task Virtualize_DispatchesExceptionsFromItemsProviderThroughRenderer()
        {
            Virtualize<int> renderedVirtualize = null;

            var rootComponent = new VirtualizeTestHostcomponent
            {
                InnerContent = BuildVirtualize(10f, AlwaysThrowsItemsProvider<int>, null, virtualize => renderedVirtualize = virtualize)
            };

            var serviceProvider = new ServiceCollection()
                .AddTransient((sp) => Mock.Of<IJSRuntime>())
                .BuildServiceProvider();

            var testRenderer = new TestRenderer(serviceProvider);
            var componentId = testRenderer.AssignRootComponentId(rootComponent);

            // Render to populate the component reference.
            await testRenderer.RenderRootComponentAsync(componentId);

            Assert.NotNull(renderedVirtualize);

            // Simulate a JS spacer callback.
            ((IVirtualizeJsCallbacks)renderedVirtualize).OnAfterSpacerVisible(10f, 50f, 100f);

            // Validate that the exception is dispatched through the renderer.
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await testRenderer.RenderRootComponentAsync(componentId));
            Assert.Equal("Thrown from items provider.", ex.Message);
        }

        private ValueTask<ItemsProviderResult<TItem>> EmptyItemsProvider<TItem>(ItemsProviderRequest request)
            => ValueTask.FromResult(new ItemsProviderResult<TItem>(Enumerable.Empty<TItem>(), 0));

        private ValueTask<ItemsProviderResult<TItem>> AlwaysThrowsItemsProvider<TItem>(ItemsProviderRequest request)
            => throw new InvalidOperationException("Thrown from items provider.");

        private RenderFragment BuildVirtualize<TItem>(
            float itemSize,
            ItemsProviderDelegate<TItem> itemsProvider,
            ICollection<TItem> items,
            Action<Virtualize<TItem>> captureRenderedVirtualize = null)
            => builder =>
        {
            builder.OpenComponent<Virtualize<TItem>>(0);
            builder.AddAttribute(1, "ItemSize", itemSize);
            builder.AddAttribute(2, "ItemsProvider", itemsProvider);
            builder.AddAttribute(3, "Items", items);

            if (captureRenderedVirtualize != null)
            {
                builder.AddComponentReferenceCapture(4, component => captureRenderedVirtualize(component as Virtualize<TItem>));
            }

            builder.CloseComponent();
        };

        private class VirtualizeTestHostcomponent : AutoRenderComponent
        {
            public RenderFragment InnerContent { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "style", "overflow: auto; height: 800px;");
                builder.AddContent(2, InnerContent);
                builder.CloseElement();
            }
        }
    }
}
