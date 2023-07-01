using System.Xml.Linq;

namespace Microsoft.AspNetCore.Server.IntegrationTesting.IIS;

public static class WebConfigHelpers
{
    public static Action<XElement, string> AddOrModifyAspNetCoreSection(string key, string value)
    {
        return (element, _) =>
        {
            element
                .Descendants("system.webServer")
                .Single()
                .GetOrAdd("aspNetCore")
                .SetAttributeValue(key, value);
        };
    }

    public static Action<XElement, string> AddOrModifyHandlerSection(string key, string value)
    {
        return (element, _) =>
        {
            element
                .Descendants("system.webServer")
                .Single()
                .GetOrAdd("handlers")
                .GetOrAdd("add")
                .SetAttributeValue(key, value);
        };
    }
}
