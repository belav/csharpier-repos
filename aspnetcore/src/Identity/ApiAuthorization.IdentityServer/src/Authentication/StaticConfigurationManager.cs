﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

internal class StaticConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
{
    private readonly Task<OpenIdConnectConfiguration> _configuration;

    public StaticConfigurationManager(OpenIdConnectConfiguration configuration) => _configuration = Task.FromResult(configuration);

    public Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel) => _configuration;

    public void RequestRefresh()
    {
    }
}
