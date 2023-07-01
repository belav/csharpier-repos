using System.IO.Pipelines;

namespace Microsoft.AspNetCore.Http.Connections.Client.Internal;

internal static class ClientPipeOptions
{
    public static PipeOptions DefaultOptions = new PipeOptions(
        writerScheduler: PipeScheduler.ThreadPool,
        readerScheduler: PipeScheduler.ThreadPool,
        useSynchronizationContext: false
    );
}
