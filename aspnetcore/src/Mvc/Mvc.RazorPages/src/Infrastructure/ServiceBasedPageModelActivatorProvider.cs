using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

/// <summary>
/// <see cref="IPageActivatorProvider"/> that uses type activation to create Razor Page instances.
/// </summary>
public class ServiceBasedPageModelActivatorProvider : IPageModelActivatorProvider
{
    /// <inheritdoc/>
    public Func<PageContext, object> CreateActivator(CompiledPageActionDescriptor descriptor)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        var modelType = descriptor.ModelTypeInfo?.AsType();
        if (modelType == null)
        {
            throw new ArgumentException(
                Resources.FormatPropertyOfTypeCannotBeNull(
                    nameof(descriptor.ModelTypeInfo),
                    nameof(descriptor)
                ),
                nameof(descriptor)
            );
        }

        return context =>
        {
            return context.HttpContext.RequestServices.GetRequiredService(modelType);
        };
    }

    /// <inheritdoc/>
    public Action<PageContext, object>? CreateReleaser(CompiledPageActionDescriptor descriptor)
    {
        return null;
    }
}
