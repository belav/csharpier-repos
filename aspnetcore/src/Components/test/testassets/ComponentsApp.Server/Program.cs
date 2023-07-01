using Microsoft.AspNetCore;

namespace ComponentsApp.Server;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

    public static IWebHost BuildWebHost(string[] args) => CreateWebHostBuilder(args).Build();
}
