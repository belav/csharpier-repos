using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

internal interface IComponentRenderer
{
    ValueTask<IHtmlContent> RenderComponentAsync(
        ViewContext viewContext,
        Type componentType,
        RenderMode renderMode,
        object parameters
    );
}
