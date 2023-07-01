using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ApplicationModels;

/// <summary>
/// Model that has a list of <see cref="IFilterMetadata"/>.
/// </summary>
public interface IFilterModel
{
    /// <summary>
    /// List of <see cref="IFilterMetadata"/>.
    /// </summary>
    IList<IFilterMetadata> Filters { get; }
}
