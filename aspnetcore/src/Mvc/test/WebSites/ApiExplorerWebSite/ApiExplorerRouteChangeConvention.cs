using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ApiExplorerWebSite;

public class ApiExplorerRouteChangeConvention : Attribute, IActionModelConvention
{
    public ApiExplorerRouteChangeConvention(WellKnownChangeToken changeToken)
    {
        ChangeToken = changeToken;
    }

    public WellKnownChangeToken ChangeToken { get; }

    public void Apply(ActionModel action)
    {
        if (
            action.Attributes.OfType<ReloadAttribute>().Any()
            && ChangeToken.TokenSource.IsCancellationRequested
        )
        {
            action.ActionName = "NewIndex";
            action.Selectors.Clear();
            action.Selectors.Add(
                new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel { Template = "NewIndex" }
                }
            );
        }
    }
}
