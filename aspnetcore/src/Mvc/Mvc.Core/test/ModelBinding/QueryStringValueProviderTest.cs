using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

public class QueryStringValueProviderTest : EnumerableValueProviderTest
{
    protected override IEnumerableValueProvider GetEnumerableValueProvider(
        BindingSource bindingSource,
        Dictionary<string, StringValues> values,
        CultureInfo culture
    )
    {
        var backingStore = new QueryCollection(values);
        return new QueryStringValueProvider(bindingSource, backingStore, culture);
    }
}
