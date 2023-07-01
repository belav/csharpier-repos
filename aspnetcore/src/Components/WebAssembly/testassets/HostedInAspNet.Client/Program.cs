using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HostedInAspNet.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<Home>("app");

        await builder.Build().RunAsync();
    }
}
