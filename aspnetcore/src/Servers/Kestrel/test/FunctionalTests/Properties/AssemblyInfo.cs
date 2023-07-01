using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
#if MACOS
using Xunit;
#endif

[assembly: ShortClassName]
[assembly: LogLevel(LogLevel.Trace)]
#if MACOS
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif
