﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace BasicWebSite;

public class StoreIntoTempDataActionResult : IActionResult
{
    public Task ExecuteResultAsync(ActionContext context)
    {
        // store information in temp data
        var httpContext = context.HttpContext;
        var tempDataDictionaryFactory = httpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
        var tempDataDictionary = tempDataDictionaryFactory.GetTempData(httpContext);
        tempDataDictionary["Name"] = "Michael";

        return httpContext.Response.WriteAsync($"Hello from {nameof(StoreIntoTempDataActionResult)}");
    }
}
