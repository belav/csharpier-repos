using System;

namespace Microsoft.Extensions.CommandLineUtils;

internal sealed class CommandParsingException : Exception
{
    public CommandParsingException(CommandLineApplication command, string message)
        : base(message)
    {
        Command = command;
    }

    public CommandLineApplication Command { get; }
}
