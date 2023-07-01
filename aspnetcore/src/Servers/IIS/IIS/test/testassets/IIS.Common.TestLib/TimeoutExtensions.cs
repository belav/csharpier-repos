using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.IntegrationTesting;

public static class TimeoutExtensions
{
    public static TimeSpan DefaultTimeoutValue = TimeSpan.FromMinutes(10);
}
