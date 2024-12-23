// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BasicLinkedApp;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<HelloWorldMiddleware>();
    }
}

public class HelloWorldMiddleware
{
    public HelloWorldMiddleware(RequestDelegate next) { }

    public Task InvokeAsync(HttpContext context)
    {
        return context.Response.WriteAsync("Hello World");
    }
}
