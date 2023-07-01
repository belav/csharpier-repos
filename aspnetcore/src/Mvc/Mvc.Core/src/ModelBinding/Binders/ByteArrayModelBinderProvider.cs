using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

/// <summary>
/// An <see cref="IModelBinderProvider"/> for binding base64 encoded byte arrays.
/// </summary>
public class ByteArrayModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(byte[]))
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new ByteArrayModelBinder(loggerFactory);
        }

        return null;
    }
}
