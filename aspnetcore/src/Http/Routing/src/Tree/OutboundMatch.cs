using Microsoft.AspNetCore.Routing.Template;

namespace Microsoft.AspNetCore.Routing.Tree;

/// <summary>
/// A candidate match for link generation in a <see cref="TreeRouter"/>.
/// </summary>
public class OutboundMatch
{
    /// <summary>
    /// Gets or sets the <see cref="OutboundRouteEntry"/>.
    /// </summary>
    public OutboundRouteEntry Entry { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="TemplateBinder"/>.
    /// </summary>
    public TemplateBinder TemplateBinder { get; set; }
}
