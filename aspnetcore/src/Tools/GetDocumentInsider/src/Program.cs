using Microsoft.Extensions.ApiDescription.Tool.Commands;
using Microsoft.Extensions.Tools.Internal;

namespace Microsoft.Extensions.ApiDescription.Tool;

internal sealed class Program : ProgramBase
{
    public Program(IConsole console)
        : base(console) { }

    private static int Main(string[] args)
    {
        DebugHelper.HandleDebugSwitch(ref args);

        var console = GetConsole();

        return new Program(console).Run(
            args,
            new GetDocumentCommand(console),
            throwOnUnexpectedArg: true
        );
    }
}
