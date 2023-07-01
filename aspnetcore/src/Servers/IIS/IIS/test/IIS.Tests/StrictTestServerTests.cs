using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

public class StrictTestServerTests : LoggedTest
{
    public override void Dispose()
    {
        base.Dispose();

        if (
            TestSink.Writes.FirstOrDefault(w => w.LogLevel > LogLevel.Information)
            is WriteContext writeContext
        )
        {
            throw new XunitException($"Unexpected log: {writeContext}");
        }
    }

    protected static TaskCompletionSource<bool> CreateTaskCompletionSource()
    {
        return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
