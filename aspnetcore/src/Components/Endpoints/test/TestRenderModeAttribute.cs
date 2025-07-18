// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Components.Endpoints.Tests;

internal class TestRenderModeAttribute<T> : RenderModeAttribute
    where T : IComponentRenderMode, new()
{
    public override IComponentRenderMode Mode { get; } = new T();
}
