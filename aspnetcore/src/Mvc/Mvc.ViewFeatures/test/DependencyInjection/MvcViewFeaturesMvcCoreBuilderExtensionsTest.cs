﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public class MvcViewFeaturesMvcCoreBuilderExtensionsTest
{
    [Fact]
    public void AddViews_RegistersExpectedTempDataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddViews();

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProvider_RegistersExpectedTempDataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider();

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProvider_DoesNotRegisterOptionsConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider();

        // Assert
        Assert.DoesNotContain(
            services,
            item => item.ServiceType == typeof(IConfigureOptions<CookieTempDataProviderOptions>)
        );
    }

    [Fact]
    public void AddCookieTempDataProviderWithSetupAction_RegistersExpectedTempDataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider(options => { });

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProviderWithSetupAction_RegistersOptionsConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider(options => { });

        // Assert
        Assert.Single(
            services,
            item => item.ServiceType == typeof(IConfigureOptions<CookieTempDataProviderOptions>)
        );
    }

    [Fact]
    public void AddCookieTempDataProvider_RegistersExpectedTempDataProvider_IfCalledBeforeAddViews()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider();
        builder.AddViews();

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProviderWithSetupAction_RegistersExpectedTempDataProvider_IfCalledBeforeAddViews()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider(options => { });
        builder.AddViews();

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProvider_RegistersExpectedTempDataProvider_IfCalledAfterAddViews()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddViews();
        builder.AddCookieTempDataProvider();

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProviderWithSetupAction_RegistersExpectedTempDataProvider_IfCalledAfterAddViews()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddViews();
        builder.AddCookieTempDataProvider(options => { });

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProvider_RegistersExpectedTempDataProvider_IfCalledTwice()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider();
        builder.AddCookieTempDataProvider();

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCookieTempDataProviderWithSetupAction_RegistersExpectedTempDataProvider_IfCalledTwice()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddMvcCore();

        // Act
        builder.AddCookieTempDataProvider(options => { });
        builder.AddCookieTempDataProvider(options => { });

        // Assert
        var descriptor = Assert.Single(
            services,
            item => item.ServiceType == typeof(ITempDataProvider)
        );
        Assert.Equal(typeof(CookieTempDataProvider), descriptor.ImplementationType);
    }
}
