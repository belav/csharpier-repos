using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

[assembly: ShortClassName]
[assembly: LogLevel(LogLevel.Trace)]
// AddressRegistrationTests can cause issues with other tests so run all tests in sequence.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
