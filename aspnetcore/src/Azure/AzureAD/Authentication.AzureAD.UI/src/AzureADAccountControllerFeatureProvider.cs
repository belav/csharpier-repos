// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.AspNetCore.Authentication.AzureAD.UI.AzureAD.Controllers.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Microsoft.AspNetCore.Authentication.AzureAD.UI;

[Obsolete("This is obsolete and will be removed in a future version. Use Microsoft.Identity.Web instead. See https://aka.ms/ms-identity-web.")]
internal sealed class AzureADAccountControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>, IApplicationFeatureProvider
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (!feature.Controllers.Contains(typeof(AccountController).GetTypeInfo()))
        {
            feature.Controllers.Add(typeof(AccountController).GetTypeInfo());
        }
    }
}
