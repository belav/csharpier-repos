using Microsoft.AspNetCore.Testing;

[assembly: OSSkipCondition(OperatingSystems.MacOSX)]
[assembly: OSSkipCondition(OperatingSystems.Linux)]
