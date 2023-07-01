using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

internal static class TagHelpersAsServices
{
    public static void AddTagHelpersAsServices(
        ApplicationPartManager manager,
        IServiceCollection services
    )
    {
        if (manager == null)
        {
            throw new ArgumentNullException(nameof(manager));
        }

        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var feature = new TagHelperFeature();
        manager.PopulateFeature(feature);

        foreach (var type in feature.TagHelpers.Select(t => t.AsType()))
        {
            services.TryAddTransient(type, type);
        }

        services.Replace(
            ServiceDescriptor.Transient<ITagHelperActivator, ServiceBasedTagHelperActivator>()
        );
    }
}
