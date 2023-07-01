using Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Filters;

internal sealed class PageSaveTempDataPropertyFilterFactory : IFilterFactory
{
    public PageSaveTempDataPropertyFilterFactory(IReadOnlyList<LifecycleProperty> properties)
    {
        Properties = properties;
    }

    public IReadOnlyList<LifecycleProperty> Properties { get; }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        var service = serviceProvider.GetRequiredService<PageSaveTempDataPropertyFilter>();
        service.Properties = Properties;

        return service;
    }
}
