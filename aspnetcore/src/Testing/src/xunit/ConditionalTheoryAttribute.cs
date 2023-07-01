using System;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Testing;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer(
    "Microsoft.AspNetCore.Testing." + nameof(ConditionalTheoryDiscoverer),
    "Microsoft.AspNetCore.Testing"
)]
public class ConditionalTheoryAttribute : TheoryAttribute { }
