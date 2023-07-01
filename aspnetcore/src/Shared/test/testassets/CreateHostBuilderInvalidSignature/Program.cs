using MockHostTypes;

namespace CreateHostBuilderInvalidSignature;

public class Program
{
    public static void Main(string[] args)
    {
        var webHost = CreateHostBuilder(null, args).Build();
    }

    // Extra parameter
    private static IHostBuilder CreateHostBuilder(object extraParam, string[] args) => null;
}
