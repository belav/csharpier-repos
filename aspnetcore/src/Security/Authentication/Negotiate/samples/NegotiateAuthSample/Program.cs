using Microsoft.AspNetCore.Server.HttpSys;

namespace NegotiateAuthSample;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (
                    string.Equals(
                        "HttpSys",
                        webBuilder.GetSetting("server"),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    webBuilder.UseHttpSys(options =>
                    {
                        options.Authentication.AllowAnonymous = true;
                        options.Authentication.Schemes = AuthenticationSchemes.Negotiate;
                    });
                }
                webBuilder.UseStartup<Startup>();
            });
}
