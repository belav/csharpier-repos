using System.Globalization;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// An <see cref="IValueProviderFactory"/> for <see cref="JQueryQueryStringValueProvider"/>.
/// </summary>
public class JQueryQueryStringValueProviderFactory : IValueProviderFactory
{
    /// <inheritdoc />
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var query = context.ActionContext.HttpContext.Request.Query;
        if (query != null && query.Count > 0)
        {
            var valueProvider = new JQueryQueryStringValueProvider(
                BindingSource.Query,
                JQueryKeyValuePairNormalizer.GetValues(query, query.Count),
                CultureInfo.InvariantCulture
            );

            context.ValueProviders.Add(valueProvider);
        }

        return Task.CompletedTask;
    }
}
