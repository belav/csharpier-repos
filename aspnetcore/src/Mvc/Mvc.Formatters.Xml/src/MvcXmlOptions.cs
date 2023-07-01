using System.Collections;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.Formatters.Xml;

/// <summary>
/// Provides configuration for XML formatters.
/// </summary>
/// <remarks>This class is currently empty.</remarks>
public class MvcXmlOptions : IEnumerable<ICompatibilitySwitch>
{
    private readonly IReadOnlyList<ICompatibilitySwitch> _switches =
        Array.Empty<ICompatibilitySwitch>();

    IEnumerator<ICompatibilitySwitch> IEnumerable<ICompatibilitySwitch>.GetEnumerator() =>
        _switches.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _switches.GetEnumerator();
}
