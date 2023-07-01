using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RoutingWebSite;

public class ControllerRouteTokenTransformerConvention : RouteTokenTransformerConvention
{
    private readonly Type _controllerType;

    public ControllerRouteTokenTransformerConvention(
        Type controllerType,
        IOutboundParameterTransformer parameterTransformer
    )
        : base(parameterTransformer)
    {
        if (parameterTransformer == null)
        {
            throw new ArgumentNullException(nameof(parameterTransformer));
        }

        _controllerType = controllerType;
    }

    protected override bool ShouldApply(ActionModel action)
    {
        return action.Controller.ControllerType == _controllerType;
    }
}
