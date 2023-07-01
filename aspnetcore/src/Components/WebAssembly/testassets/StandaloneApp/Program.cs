using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace StandaloneApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("app");
        builder.Services.AddSingleton(
            new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
        );

        await builder.Build().RunAsync();
    }
}
