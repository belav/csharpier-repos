using System;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Testing;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer(
    "Microsoft.AspNetCore.Testing." + nameof(ConditionalFactDiscoverer),
    "Microsoft.AspNetCore.Testing"
)]
public class ConditionalFactAttribute : FactAttribute { }
