using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

/// <summary>
/// An <see cref="IModelBinderProvider"/> for binding types that have a TryParse method.
/// </summary>
public sealed class TryParseModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.IsParseableType)
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new TryParseModelBinder(context.Metadata.ModelType, loggerFactory);
        }

        return null;
    }
}
