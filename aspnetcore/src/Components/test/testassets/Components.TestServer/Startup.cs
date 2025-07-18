// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace TestServer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<TestAppInfo>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var enUs = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = enUs;
        CultureInfo.DefaultThreadCurrentUICulture = enUs;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Run(async ctx =>
        {
            var appsInfo = ctx.RequestServices.GetRequiredService<TestAppInfo>();
            var response = ctx.Response.ContentType = "text/html;charset=utf-8";
            using var writer = new StringWriter();
            await writer.WriteAsync(
                @"<!DOCTYPE html>
<html>
  <head>
    <title>Blazor test server index</title>
  </head>
  <body>
    <table>
      <tr>
        <th>Scenario</th>
        <th>
          <Link>Link</Link>
        </th>
      </tr>
"
            );
            foreach (var scenario in appsInfo.Scenarios)
            {
                await writer.WriteAsync(
                    @$"
      <tr>
        <td>{scenario.Key}</td>
        <td><a href=""{scenario.Value}"">{scenario.Value}</a></td>
      </tr>
"
                );
            }
            await writer.WriteAsync(
                @"
    </table>
    <style>
        table, th, td, tr { border: 1px solid black; }
        th { font-weight: bold; }
    <style>
  </body>
</html>"
            );
            var content = writer.ToString();
            ctx.Response.ContentLength = content.Length;
            await ctx.Response.WriteAsync(content);
        });
    }
}
