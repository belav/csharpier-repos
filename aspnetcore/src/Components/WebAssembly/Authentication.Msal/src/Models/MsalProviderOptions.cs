// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Authentication.WebAssembly.Msal.Models
{
    /// <summary>
    /// Authentication provider options for the msal.js authentication provider.
    /// </summary>
    public class MsalProviderOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="MsalAuthenticationOptions"/> to use for authentication operations.
        /// </summary>
        [JsonPropertyName("auth")]
        public MsalAuthenticationOptions Authentication { get; set; } = new MsalAuthenticationOptions
        {
            RedirectUri = "authentication/login-callback",
            PostLogoutRedirectUri = "authentication/logout-callback"
        };

        /// <summary>
        /// Gets or sets the msal.js cache options.
        /// </summary>
        public MsalCacheOptions Cache { get; set; } = new MsalCacheOptions
        {
            // This matches the defaults in msal.js
            CacheLocation = "sessionStorage",
            StoreAuthStateInCookie = false
        };

        /// <summary>
        /// Gets or set the list of default access tokens scopes to provision during the sign-in flow.
        /// </summary>
        public IList<string> DefaultAccessTokenScopes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a list of additional scopes to consent during the initial sign-in flow.
        /// </summary>
        /// <remarks>
        /// Use this parameter to request consent for scopes for other resources.
        /// </remarks>
        public IList<string> AdditionalScopesToConsent { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the login mode that is used when initiating the sign-in flow.
        /// </summary>
        public string LoginMode { get; set; } = "popup";
    }
}
