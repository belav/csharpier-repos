using System.Diagnostics;

namespace Microsoft.AspNetCore.Mvc.TagHelpers;

internal sealed class CurrentValues
{
    public CurrentValues(ICollection<string> values)
    {
        Debug.Assert(values != null);
        Values = values;
    }

    public ICollection<string> Values { get; }

    public ICollection<string> ValuesAndEncodedValues { get; set; }
}
