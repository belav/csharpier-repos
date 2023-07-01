using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.Authorization;

/// <summary>
/// A filter that allows anonymous requests, disabling some <see cref="IAuthorizationFilter"/>s.
/// </summary>
public interface IAllowAnonymousFilter : IFilterMetadata { }
