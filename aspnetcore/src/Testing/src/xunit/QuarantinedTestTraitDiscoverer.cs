using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

// Do not change this namespace without changing the usage in QuarantinedTestAttribute
namespace Microsoft.AspNetCore.Testing;

public class QuarantinedTestTraitDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        if (traitAttribute is ReflectionAttributeInfo { Attribute: QuarantinedTestAttribute })
        {
            yield return new KeyValuePair<string, string>("Quarantined", "true");
        }
        else
        {
            throw new InvalidOperationException(
                "The 'QuarantinedTest' attribute is only supported via reflection."
            );
        }
    }
}
