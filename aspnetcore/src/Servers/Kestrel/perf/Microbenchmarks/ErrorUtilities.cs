using System;

namespace Microsoft.AspNetCore.Server.Kestrel.Microbenchmarks;

public static class ErrorUtilities
{
    public static void ThrowInvalidRequestLine()
    {
        throw new InvalidOperationException("Invalid request line");
    }

    public static void ThrowInvalidRequestHeaders()
    {
        throw new InvalidOperationException("Invalid request headers");
    }
}
