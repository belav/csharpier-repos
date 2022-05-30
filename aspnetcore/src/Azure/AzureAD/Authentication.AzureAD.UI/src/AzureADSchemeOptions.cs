// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Authentication.AzureAD.UI;

internal sealed class AzureADSchemeOptions
{
    public IDictionary<string, AzureADOpenIDSchemeMapping> OpenIDMappings { get; set; } = new Dictionary<string, AzureADOpenIDSchemeMapping>();

    public IDictionary<string, JwtBearerSchemeMapping> JwtBearerMappings { get; set; } = new Dictionary<string, JwtBearerSchemeMapping>();

    public sealed class AzureADOpenIDSchemeMapping
    {
        public string OpenIdConnectScheme { get; set; }
        public string CookieScheme { get; set; }
    }

    public sealed class JwtBearerSchemeMapping
    {
        public string JwtBearerScheme { get; set; }
    }
}
