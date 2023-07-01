using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

internal sealed class NullView : IView
{
    public static readonly NullView Instance = new NullView();

    public string Path => string.Empty;

    public Task RenderAsync(ViewContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return Task.CompletedTask;
    }
}
