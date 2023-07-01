using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Microsoft.AspNetCore.Mvc.Razor;

public abstract class RazorPage<TModel> : RazorPage
{
    public TModel Model { get; }

    public ViewDataDictionary<TModel> ViewData { get; set; }
}
