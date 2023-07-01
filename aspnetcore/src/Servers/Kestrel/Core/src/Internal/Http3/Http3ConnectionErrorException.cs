using System.Net.Http;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http3;

internal sealed class Http3ConnectionErrorException : Exception
{
    public Http3ConnectionErrorException(string message, Http3ErrorCode errorCode)
        : base(
            $"HTTP/3 connection error ({Http3Formatting.ToFormattedErrorCode(errorCode)}): {message}"
        )
    {
        ErrorCode = errorCode;
    }

    public Http3ErrorCode ErrorCode { get; }
}
