// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Authentication.OpenIdConnect
{
    /// <summary>
    /// A context for <see cref="OpenIdConnectEvents.TokenValidated(TokenValidatedContext)"/>.
    /// </summary>
    public class TokenValidatedContext : RemoteAuthenticationContext<OpenIdConnectOptions>
    {
        /// <summary>
        /// Creates a <see cref="TokenValidatedContext"/>
        /// </summary>
        /// <inheritdoc />
        public TokenValidatedContext(HttpContext context, AuthenticationScheme scheme, OpenIdConnectOptions options, ClaimsPrincipal principal, AuthenticationProperties properties)
            : base(context, scheme, options, properties)
            => Principal = principal;

        /// <summary>
        /// Gets or sets the <see cref="OpenIdConnectMessage"/>.
        /// </summary>
        public OpenIdConnectMessage ProtocolMessage { get; set; } = default!;

        /// <summary>
        /// Gets or sets the validated security token.
        /// </summary>
        public JwtSecurityToken SecurityToken { get; set; } = default!;

        /// <summary>
        /// Gets or sets the token endpoint response.
        /// </summary>
        public OpenIdConnectMessage? TokenEndpointResponse { get; set; }

        /// <summary>
        /// Gets or sets the protocol nonce.
        /// </summary>
        public string? Nonce { get; set; }
    }
}
