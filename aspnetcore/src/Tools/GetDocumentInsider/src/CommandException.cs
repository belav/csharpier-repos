using System;

namespace Microsoft.Extensions.ApiDescription.Tool;

internal sealed class CommandException : Exception
{
    public CommandException(string message)
        : base(message) { }

    public CommandException(string message, Exception innerException)
        : base(message, innerException) { }
}
