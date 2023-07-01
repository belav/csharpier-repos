using Microsoft.AspNetCore.Mvc.Routing;

namespace VersioningWebSite;

public class VersionGetAttribute : VersionRouteAttribute, IActionHttpMethodProvider
{
    public VersionGetAttribute(string template)
        : base(template) { }

    public VersionGetAttribute(string template, string versionRange)
        : base(template, versionRange) { }

    private readonly IEnumerable<string> _httpMethods = new[] { "GET" };

    public IEnumerable<string> HttpMethods
    {
        get { return _httpMethods; }
    }
}
