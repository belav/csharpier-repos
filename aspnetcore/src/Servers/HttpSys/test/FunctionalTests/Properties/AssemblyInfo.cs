using Microsoft.AspNetCore.Testing;
using Xunit;

[assembly: OSSkipCondition(OperatingSystems.MacOSX)]
[assembly: OSSkipCondition(OperatingSystems.Linux)]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
