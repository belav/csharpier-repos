using System.Net.Http;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http3;

internal sealed class Http3StreamErrorException : Exception
{
    public Http3StreamErrorException(string message, Http3ErrorCode errorCode)
        : base($"HTTP/3 stream error ({errorCode}): {message}")
    {
        ErrorCode = errorCode;
    }

    public Http3ErrorCode ErrorCode { get; }
}
