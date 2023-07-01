using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// A marker interface for filters which define a policy for maximum size for the request body.
/// </summary>
public interface IRequestSizePolicy : IFilterMetadata { }
