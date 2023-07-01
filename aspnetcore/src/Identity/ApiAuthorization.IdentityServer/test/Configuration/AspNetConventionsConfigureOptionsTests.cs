﻿using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

public class AspNetConventionsConfigureOptionsTests
{
    [Fact]
    public void Configure_SetsUpIdentityPathsAndCookie()
    {
        // Arrange
        var options = new IdentityServerOptions();
        var configure = new AspNetConventionsConfigureOptions();

        // Act
        configure.Configure(options);

        // Assert
        Assert.Equal(
            IdentityConstants.ApplicationScheme,
            options.Authentication.CookieAuthenticationScheme
        );
    }

    [Fact]
    public void Configure_SetsUpIdentityServerEvents()
    {
        // Arrange
        var options = new IdentityServerOptions();
        var configure = new AspNetConventionsConfigureOptions();

        // Act
        configure.Configure(options);

        // Assert
        Assert.True(options.Events.RaiseErrorEvents);
        Assert.True(options.Events.RaiseInformationEvents);
        Assert.True(options.Events.RaiseFailureEvents);
        Assert.True(options.Events.RaiseSuccessEvents);
    }
}
