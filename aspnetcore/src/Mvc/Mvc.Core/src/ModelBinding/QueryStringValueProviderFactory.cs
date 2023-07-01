using System.Globalization;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// A <see cref="IValueProviderFactory"/> that creates <see cref="IValueProvider"/> instances that
/// read values from the request query-string.
/// </summary>
public class QueryStringValueProviderFactory : IValueProviderFactory
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
            var valueProvider = new QueryStringValueProvider(
                BindingSource.Query,
                query,
                CultureInfo.InvariantCulture
            );

            context.ValueProviders.Add(valueProvider);
        }

        return Task.CompletedTask;
    }
}
