using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.SignalR.Internal;

namespace Microsoft.AspNetCore.SignalR.Microbenchmarks;

public class TypedClientBuilderBenchmark
{
    private static readonly IClientProxy Dummy = new DummyProxy();

    [Benchmark]
    public ITestClient Build()
    {
        return TypedClientBuilder<ITestClient>.Build(Dummy);
    }

    public interface ITestClient { }

    private sealed class DummyProxy : IClientProxy
    {
        public Task SendCoreAsync(
            string method,
            object[] args,
            CancellationToken cancellationToken = default
        )
        {
            return Task.CompletedTask;
        }
    }
}
