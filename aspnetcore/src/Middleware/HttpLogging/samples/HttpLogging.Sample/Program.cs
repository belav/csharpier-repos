using System.Text.Json;

namespace HttpLogging.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                // Json Logging
                logging.ClearProviders();
                logging.AddJsonConsole(options =>
                {
                    options.JsonWriterOptions = new JsonWriterOptions() { Indented = true };
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
