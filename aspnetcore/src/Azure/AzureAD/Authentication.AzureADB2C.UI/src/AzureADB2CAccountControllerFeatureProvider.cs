using System.Reflection;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI.AzureADB2C.Controllers.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Microsoft.AspNetCore.Authentication.AzureADB2C.UI;

[Obsolete(
    "This is obsolete and will be removed in a future version. Use Microsoft.Identity.Web instead. See https://aka.ms/ms-identity-web."
)]
internal sealed class AzureADB2CAccountControllerFeatureProvider
    : IApplicationFeatureProvider<ControllerFeature>,
        IApplicationFeatureProvider
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (!feature.Controllers.Contains(typeof(AccountController).GetTypeInfo()))
        {
            feature.Controllers.Add(typeof(AccountController).GetTypeInfo());
        }
    }
}
