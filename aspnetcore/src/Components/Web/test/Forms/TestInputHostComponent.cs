// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Test.Helpers;

namespace Microsoft.AspNetCore.Components.Forms
{
    internal class TestInputHostComponent<TValue, TComponent> : AutoRenderComponent where TComponent : InputBase<TValue>
    {
        public Dictionary<string, object> AdditionalAttributes { get; set; }

        public EditContext EditContext { get; set; }

        public TValue Value { get; set; }

        public Action<TValue> ValueChanged { get; set; }

        public Expression<Func<TValue>> ValueExpression { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddAttribute(1, "Value", EditContext);
            builder.AddAttribute(2, "ChildContent", new RenderFragment(childBuilder =>
            {
                childBuilder.OpenComponent<TComponent>(0);
                childBuilder.AddAttribute(0, "Value", Value);
                childBuilder.AddAttribute(1, "ValueChanged",
                    EventCallback.Factory.Create(this, ValueChanged));
                childBuilder.AddAttribute(2, "ValueExpression", ValueExpression);
                childBuilder.AddMultipleAttributes(3, AdditionalAttributes);
                childBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }
}
