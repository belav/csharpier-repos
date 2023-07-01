using System;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.DataProtection;

internal class TestRedisServerIsAvailableAttribute : Attribute, ITestCondition
{
    public bool IsMet => !string.IsNullOrEmpty(TestRedisServer.GetConnectionString());

    public string SkipReason =>
        $"A test redis server must be configured to run. Set the connection string as an environment variable as {TestRedisServer.ConnectionStringKeyName.Replace(":", "__")} or in testconfig.json";
}
