using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Wasm.Performance.TestApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("app");

        await builder.Build().RunAsync();
    }
}
