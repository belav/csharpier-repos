using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Components.Rendering;

internal readonly struct ComponentRenderedText
{
    public ComponentRenderedText(int componentId, IHtmlContent htmlContent)
    {
        ComponentId = componentId;
        HtmlContent = htmlContent;
    }

    public int ComponentId { get; }

    public IHtmlContent HtmlContent { get; }
}
