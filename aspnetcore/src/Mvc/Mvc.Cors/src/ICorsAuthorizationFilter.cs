using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.Cors;

/// <summary>
/// A filter that can be used to enable/disable CORS support for a resource.
/// </summary>
internal interface ICorsAuthorizationFilter : IAsyncAuthorizationFilter, IOrderedFilter { }
