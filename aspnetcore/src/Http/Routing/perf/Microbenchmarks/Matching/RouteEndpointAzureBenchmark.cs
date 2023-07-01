using BenchmarkDotNet.Attributes;

namespace Microsoft.AspNetCore.Routing.Matching;

public class RouteEndpointAzureBenchmark : MatcherAzureBenchmarkBase
{
    [Benchmark]
    public void CreateEndpoints()
    {
        SetupEndpoints();
    }
}
