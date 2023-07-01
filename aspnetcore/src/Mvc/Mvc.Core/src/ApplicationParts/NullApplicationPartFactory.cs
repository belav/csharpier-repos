using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.ApplicationParts;

/// <summary>
/// An <see cref="ApplicationPartFactory"/> that produces no parts.
/// <para>
/// This factory may be used to to preempt Mvc's default part discovery allowing for custom configuration at a later stage.
/// </para>
/// </summary>
public class NullApplicationPartFactory : ApplicationPartFactory
{
    /// <inheritdoc />
    public override IEnumerable<ApplicationPart> GetApplicationParts(Assembly assembly)
    {
        return Enumerable.Empty<ApplicationPart>();
    }
}
