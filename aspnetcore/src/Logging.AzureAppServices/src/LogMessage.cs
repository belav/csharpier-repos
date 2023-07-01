using System;

namespace Microsoft.Extensions.Logging.AzureAppServices;

internal readonly struct LogMessage
{
    public LogMessage(DateTimeOffset timestamp, string message)
    {
        Timestamp = timestamp;
        Message = message;
    }

    public DateTimeOffset Timestamp { get; }
    public string Message { get; }
}
