using System.IO.Pipelines;

namespace Microsoft.AspNetCore.Server.Kestrel.Microbenchmarks;

internal sealed class MockDuplexPipe : IDuplexPipe
{
    public MockDuplexPipe(PipeReader input, PipeWriter output)
    {
        Input = input;
        Output = output;
    }

    public PipeReader Input { get; }
    public PipeWriter Output { get; }
}
