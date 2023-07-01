using Microsoft.AspNetCore;

namespace AzureAD.WebSite;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder().UseStartup<Startup>();
    }
}
