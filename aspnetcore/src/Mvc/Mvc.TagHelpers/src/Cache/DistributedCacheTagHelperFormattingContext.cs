using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

/// <summary>
/// Represents an object containing the information to serialize with <see cref="IDistributedCacheTagHelperFormatter" />.
/// </summary>
public class DistributedCacheTagHelperFormattingContext
{
    /// <summary>
    /// Gets the <see cref="HtmlString"/> instance.
    /// </summary>
    public HtmlString Html { get; set; }
}
