// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Internal.LinkerFlags;

namespace Microsoft.AspNetCore.Components
{
    /// <summary>
    /// Optional base class for components that represent a layout.
    /// Alternatively, components may implement <see cref="IComponent"/> directly
    /// and declare their own parameter named <see cref="Body"/>.
    /// </summary>
    public abstract class LayoutComponentBase : ComponentBase
    {
        internal const string BodyPropertyName = nameof(Body);

        /// <summary>
        /// Gets the content to be rendered inside the layout.
        /// </summary>
        [Parameter]
        public RenderFragment? Body { get; set; }

        /// <inheritdoc />
        // Derived instances of LayoutComponentBase do not appear in any statically analyzable
        // calls of OpenComponent<T> where T is well-known. Consequently we have to explicitly provide a hint to the trimmer to preserve
        // properties.
        [DynamicDependency(Component, typeof(LayoutComponentBase))]
        public override Task SetParametersAsync(ParameterView parameters) => base.SetParametersAsync(parameters);
    }
}
