// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Components.WebView.Document;

internal class ComponentNode : ContainerNode
{
    public ComponentNode(int componentId)
    {
        ComponentId = componentId;
    }

    public int ComponentId { get; }
}
