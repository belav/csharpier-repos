// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Test.Helpers;
using Xunit;

namespace Microsoft.AspNetCore.Components.Forms
{
    public class InputRadioTest
    {
        [Fact]
        public async Task ThrowsOnFirstRenderIfInputRadioHasNoGroup()
        {
            var model = new TestModel();
            var rootComponent = new TestInputRadioHostComponent<TestEnum>
            {
                EditContext = new EditContext(model),
                InnerContent = RadioButtonsWithoutGroup(null)
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => RenderAndGetTestInputComponentAsync(rootComponent));
            Assert.Contains($"must have an ancestor", ex.Message);
        }

        [Fact]
        public async Task GroupGeneratesNameGuidWhenInvalidNameSupplied()
        {
            var model = new TestModel();
            var rootComponent = new TestInputRadioHostComponent<TestEnum>
            {
                EditContext = new EditContext(model),
                InnerContent = RadioButtonsWithGroup(null, () => model.TestEnum)
            };

            var inputRadioComponents = await RenderAndGetTestInputComponentAsync(rootComponent);

            Assert.All(inputRadioComponents, inputRadio => Assert.True(Guid.TryParseExact(inputRadio.GroupName, "N", out _)));
        }

        [Fact]
        public async Task RadioInputContextExistsWhenValidNameSupplied()
        {
            var groupName = "group";
            var model = new TestModel();
            var rootComponent = new TestInputRadioHostComponent<TestEnum>
            {
                EditContext = new EditContext(model),
                InnerContent = RadioButtonsWithGroup(groupName, () => model.TestEnum)
            };

            var inputRadioComponents = await RenderAndGetTestInputComponentAsync(rootComponent);

            Assert.All(inputRadioComponents, inputRadio => Assert.Equal(groupName, inputRadio.GroupName));
        }

        private static RenderFragment RadioButtonsWithoutGroup(string name) => (builder) =>
        {
            foreach (var selectedValue in (TestEnum[])Enum.GetValues(typeof(TestEnum)))
            {
                builder.OpenComponent<TestInputRadio>(0);
                builder.AddAttribute(1, "Name", name);
                builder.AddAttribute(2, "Value", selectedValue);
                builder.CloseComponent();
            }
        };

        private static RenderFragment RadioButtonsWithGroup(string name, Expression<Func<TestEnum>> valueExpression) => (builder) =>
        {
            builder.OpenComponent<InputRadioGroup<TestEnum>>(0);
            builder.AddAttribute(1, "Name", name);
            builder.AddAttribute(2, "ValueExpression", valueExpression);
            builder.AddAttribute(2, "ChildContent", new RenderFragment((childBuilder) =>
            {
                foreach (var value in (TestEnum[])Enum.GetValues(typeof(TestEnum)))
                {
                    childBuilder.OpenComponent<TestInputRadio>(0);
                    childBuilder.AddAttribute(1, "Value", value);
                    childBuilder.CloseComponent();
                }
            }));

            builder.CloseComponent();
        };

        private static IEnumerable<TestInputRadio> FindInputRadioComponents(CapturedBatch batch)
            => batch.ReferenceFrames
                    .Where(f => f.FrameType == RenderTreeFrameType.Component)
                    .Select(f => f.Component)
                    .OfType<TestInputRadio>();

        private static async Task<IEnumerable<TestInputRadio>> RenderAndGetTestInputComponentAsync(TestInputRadioHostComponent<TestEnum> rootComponent)
        {
            var testRenderer = new TestRenderer();
            var componentId = testRenderer.AssignRootComponentId(rootComponent);
            await testRenderer.RenderRootComponentAsync(componentId);
            return FindInputRadioComponents(testRenderer.Batches.Single());
        }

        private enum TestEnum
        {
            One,
            Two,
            Three
        }

        private class TestModel
        {
            public TestEnum TestEnum { get; set; }
        }

        private class TestInputRadio : InputRadio<TestEnum>
        {
            public string GroupName => Context.GroupName;
        }

        private class TestInputRadioHostComponent<TValue> : AutoRenderComponent
        {
            public EditContext EditContext { get; set; }

            public RenderFragment InnerContent { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<CascadingValue<EditContext>>(0);
                builder.AddAttribute(1, "Value", EditContext);
                builder.AddAttribute(2, "ChildContent", InnerContent);
                builder.CloseComponent();
            }
        }
    }
}
