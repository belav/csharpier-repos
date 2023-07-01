using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

/// <summary>
/// A marker interface for filters which define a policy for antiforgery token validation.
/// </summary>
public interface IAntiforgeryPolicy : IFilterMetadata { }
