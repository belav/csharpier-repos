using System;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

public class RazorPageAttribute : RazorViewAttribute
{
    public RazorPageAttribute(string path, Type viewType, string routeTemplate)
        : base(path, viewType)
    {
        RouteTemplate = routeTemplate;
    }

    public string RouteTemplate { get; }
}
