using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

namespace Microsoft.AspNetCore.Server.Kestrel.Microbenchmarks;

internal sealed class MockSystemClock : ISystemClock
{
    public DateTimeOffset UtcNow { get; }
    public long UtcNowTicks { get; }
    public DateTimeOffset UtcNowUnsynchronized { get; }
}
