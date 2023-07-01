using Microsoft.Extensions.CommandLineUtils;
using Wasm.Performance.ConsoleHost.Scenarios;

namespace Wasm.Performance.ConsoleHost;

internal sealed class Program : CommandLineApplication
{
    static void Main(string[] args)
    {
        new Program().Execute(args);
    }

    public Program()
    {
        OnExecute(() =>
        {
            ShowHelp();
            return 1;
        });

        Commands.Add(new GridScenario());
    }
}
