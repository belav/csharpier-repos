using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

public interface IViewContextAware
{
    void Contextualize(ViewContext viewContext);
}
