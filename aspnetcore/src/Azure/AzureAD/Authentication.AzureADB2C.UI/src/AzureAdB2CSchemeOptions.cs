// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Authentication.AzureADB2C.UI;

internal sealed class AzureADB2CSchemeOptions
{
    public IDictionary<string, AzureADB2COpenIDSchemeMapping> OpenIDMappings { get; set; } = new Dictionary<string, AzureADB2COpenIDSchemeMapping>();

    public IDictionary<string, JwtBearerSchemeMapping> JwtBearerMappings { get; set; } = new Dictionary<string, JwtBearerSchemeMapping>();

    public sealed class AzureADB2COpenIDSchemeMapping
    {
        public string OpenIdConnectScheme { get; set; }
        public string CookieScheme { get; set; }
    }

    public sealed class JwtBearerSchemeMapping
    {
        public string JwtBearerScheme { get; set; }
    }
}
