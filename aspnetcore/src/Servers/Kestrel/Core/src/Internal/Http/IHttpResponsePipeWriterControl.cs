using System.IO.Pipelines;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

internal interface IHttpResponsePipeWriterControl
{
    void ProduceContinue();
    Memory<byte> GetMemory(int sizeHint = 0);
    Span<byte> GetSpan(int sizeHint = 0);
    void Advance(int bytes);
    ValueTask<FlushResult> FlushPipeAsync(CancellationToken cancellationToken);
    ValueTask<FlushResult> WritePipeAsync(
        ReadOnlyMemory<byte> source,
        CancellationToken cancellationToken
    );
    void CancelPendingFlush();
}
