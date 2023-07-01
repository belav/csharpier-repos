using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Template;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer;

internal sealed class ApiParameterContext
{
    public ApiParameterContext(
        IModelMetadataProvider metadataProvider,
        ControllerActionDescriptor actionDescriptor,
        IReadOnlyList<TemplatePart> routeParameters
    )
    {
        MetadataProvider = metadataProvider;
        ActionDescriptor = actionDescriptor;
        RouteParameters = routeParameters;

        Results = new List<ApiParameterDescription>();
    }

    public ControllerActionDescriptor ActionDescriptor { get; }

    public IModelMetadataProvider MetadataProvider { get; }

    public IList<ApiParameterDescription> Results { get; }

    public IReadOnlyList<TemplatePart> RouteParameters { get; }
}
