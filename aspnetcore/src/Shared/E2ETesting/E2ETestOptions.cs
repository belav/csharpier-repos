// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable CA1810 // Initialize all static fields inline.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.E2ETesting;

public class E2ETestOptions
{
    private const string TestingOptionsPrefix = "Microsoft.AspNetCore.E2ETesting";
    public static readonly IConfiguration Configuration;

    public static E2ETestOptions Instance;

    static E2ETestOptions()
    {
        // Capture all the attributes that start with Microsoft.AspNetCore.E2ETesting and add them as a memory collection
        // to the list of settings. We use GetExecutingAssembly, this works because E2ETestOptions is shared source.
        var metadataAttributes = Assembly
            .GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(ama => ama.Key.StartsWith(TestingOptionsPrefix, StringComparison.Ordinal))
            .ToDictionary(
                kvp => kvp.Key.Substring(TestingOptionsPrefix.Length + 1),
                kvp => kvp.Value
            );

        try
        {
            // We save the configuration just to make resolved values easier to debug.
            var builder = new ConfigurationBuilder().AddInMemoryCollection(metadataAttributes);

            if (!metadataAttributes.TryGetValue("CI", out var value) || string.IsNullOrEmpty(value))
            {
                builder.AddJsonFile("e2eTestSettings.json", optional: true);
            }
            else
            {
                builder.AddJsonFile("e2eTestSettings.ci.json", optional: true);
            }

            Configuration = builder.AddEnvironmentVariables("E2ETESTS_").Build();

            var instance = new E2ETestOptions();
            Configuration.Bind(instance);
            Instance = instance;
        }
        catch { }
    }

    public int DefaultWaitTimeoutInSeconds { get; set; } = 15;

    public string ScreenShotsPath { get; set; }

    public double DefaultAfterFailureWaitTimeoutInSeconds { get; set; } = 10;

    public bool SauceTest { get; set; }

    public SauceOptions Sauce { get; set; }
}
