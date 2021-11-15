// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BasicWebSite;

public class StartupWithSystemTextJson
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMvc();

        services.AddSingleton<ContactsRepository>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints((endpoints) => endpoints.MapDefaultControllerRoute());
    }
}
