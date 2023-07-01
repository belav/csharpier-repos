using DevServerProgram = Microsoft.AspNetCore.Components.WebAssembly.DevServer.Server.Program;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Components.WebAssembly.DevServer;

internal sealed class Program
{
    static int Main(string[] args)
    {
        DevServerProgram.BuildWebHost(args).Run();
        return 0;
    }
}
