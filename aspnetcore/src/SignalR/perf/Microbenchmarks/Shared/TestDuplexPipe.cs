using System.IO.Pipelines;

namespace Microsoft.AspNetCore.SignalR.Microbenchmarks.Shared;

public class TestDuplexPipe : IDuplexPipe
{
    private readonly TestPipeReader _input;

    public PipeReader Input => _input;

    public PipeWriter Output { get; }

    public TestDuplexPipe(bool writerForceAsync = false)
    {
        _input = new TestPipeReader();
        Output = new TestPipeWriter { ForceAsync = writerForceAsync };
    }

    public void AddReadResult(ValueTask<ReadResult> readResult)
    {
        _input.ReadResults.Add(readResult);
    }
}
