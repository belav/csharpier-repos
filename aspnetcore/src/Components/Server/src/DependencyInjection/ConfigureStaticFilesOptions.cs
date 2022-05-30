// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Components.Server;

internal sealed class ConfigureStaticFilesOptions : IPostConfigureOptions<StaticFileOptions>
{
    public ConfigureStaticFilesOptions(IWebHostEnvironment environment)
    {
        Environment = environment;
    }

    public IWebHostEnvironment Environment { get; }

    public void PostConfigure(string name, StaticFileOptions options)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        options = options ?? throw new ArgumentNullException(nameof(options));

        if (name != Options.DefaultName)
        {
            return;
        }

        // Basic initialization in case the options weren't initialized by any other component
        options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
        if (options.FileProvider == null && Environment.WebRootFileProvider == null)
        {
            throw new InvalidOperationException("Missing FileProvider.");
        }

        options.FileProvider = options.FileProvider ?? Environment.WebRootFileProvider;

        var prepareResponse = options.OnPrepareResponse;
        if (prepareResponse == null)
        {
            options.OnPrepareResponse = CacheHeaderSettings.SetCacheHeaders;
        }
        else
        {
            void PrepareResponse(StaticFileResponseContext context)
            {
                prepareResponse(context);
                CacheHeaderSettings.SetCacheHeaders(context);
            }

            options.OnPrepareResponse = PrepareResponse;
        }

        // Add our provider
        var provider = new ManifestEmbeddedFileProvider(typeof(ConfigureStaticFilesOptions).Assembly);

        options.FileProvider = new CompositeFileProvider(provider, options.FileProvider);
    }
}
