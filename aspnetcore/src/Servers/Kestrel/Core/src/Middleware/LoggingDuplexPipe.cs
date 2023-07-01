using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

internal sealed class LoggingDuplexPipe : DuplexPipeStreamAdapter<LoggingStream>
{
    private static readonly StreamPipeReaderOptions _defaultReaderOptions =
        new(useZeroByteReads: true);
    private static readonly StreamPipeWriterOptions _defaultWriterOptions = new();

    public LoggingDuplexPipe(IDuplexPipe transport, ILogger logger)
        : base(
            transport,
            _defaultReaderOptions,
            _defaultWriterOptions,
            stream => new LoggingStream(stream, logger)
        ) { }
}
