// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Testing;
using Microsoft.JSInterop;
using Moq;
using static Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyCultureProvider;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting;

public class WebAssemblyCultureProviderTest
{
    [Theory]
    [InlineData("fr-FR", new[] { "fr-FR", "fr" })]
    [InlineData("tzm-Latn-DZ", new[] { "tzm-Latn-DZ", "tzm-Latn", "tzm" })]
    public void GetCultures_ReturnsCultureClosure(string cultureName, string[] expected)
    {
        // Arrange
        var culture = new CultureInfo(cultureName);

        // Act
        var actual = WebAssemblyCultureProvider.GetCultures(culture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task LoadCurrentCultureResourcesAsync_ReadsAssemblies()
    {
        // Arrange
        using var cultureReplacer = new CultureReplacer("en-GB");
        var invoker = new Mock<IJSUnmarshalledRuntime>();
#pragma warning disable CS0618 // Type or member is obsolete
        invoker.Setup(i => i.InvokeUnmarshalled<string[], object, object, Task<object>>(GetSatelliteAssemblies, new[] { "en-GB", "en" }, null, null))
            .Returns(Task.FromResult<object>(1))
            .Verifiable();

        invoker.Setup(i => i.InvokeUnmarshalled<object, object, object, object[]>(ReadSatelliteAssemblies, null, null, null))
            .Returns(new object[] { File.ReadAllBytes(GetType().Assembly.Location) })
            .Verifiable();
#pragma warning restore CS0618 // Type or member is obsolete

        var loader = new WebAssemblyCultureProvider(invoker.Object, CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

        // Act
        await loader.LoadCurrentCultureResourcesAsync();

        // Assert
        invoker.Verify();
    }

    [Fact]
    public async Task LoadCurrentCultureResourcesAsync_DoesNotReadAssembliesWhenThereAreNone()
    {
        // Arrange
        using var cultureReplacer = new CultureReplacer("en-GB");
        var invoker = new Mock<IJSUnmarshalledRuntime>();
#pragma warning disable CS0618 // Type or member is obsolete
        invoker.Setup(i => i.InvokeUnmarshalled<string[], object, object, Task<object>>(GetSatelliteAssemblies, new[] { "en-GB", "en" }, null, null))
            .Returns(Task.FromResult<object>(0))
            .Verifiable();
#pragma warning restore CS0618 // Type or member is obsolete

        var loader = new WebAssemblyCultureProvider(invoker.Object, CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

        // Act
        await loader.LoadCurrentCultureResourcesAsync();

#pragma warning disable CS0618 // Type or member is obsolete
        // Assert
        invoker.Verify(i => i.InvokeUnmarshalled<object, object, object, object[]>(ReadSatelliteAssemblies, null, null, null), Times.Never());
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Fact]
    public void ThrowIfCultureChangeIsUnsupported_ThrowsIfCulturesAreDifferentAndICUShardingIsUsed()
    {
        // Arrange
        Environment.SetEnvironmentVariable("__BLAZOR_SHARDED_ICU", "1");
        try
        {
            // WebAssembly is initialized with en-US
            var cultureProvider = new WebAssemblyCultureProvider(DefaultWebAssemblyJSRuntime.Instance, new CultureInfo("en-US"), new CultureInfo("en-US"));

            // Culture is changed to fr-FR as part of the app
            using var cultureReplacer = new CultureReplacer("fr-FR");

            var ex = Assert.Throws<InvalidOperationException>(() => cultureProvider.ThrowIfCultureChangeIsUnsupported());
            Assert.Equal("Blazor detected a change in the application's culture that is not supported with the current project configuration. " +
                "To change culture dynamically during startup, set <BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData> in the application's project file.",
                ex.Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable("__BLAZOR_SHARDED_ICU", null);
        }
    }
}
