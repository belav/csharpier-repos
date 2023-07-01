using MockHostTypes;

namespace CreateHostBuilderPatternTestSite;

public class Program
{
    public static void Main(string[] args)
    {
        var webHost = CreateHostBuilder(args).Build();
    }

    // Do not change the signature of this method. It's used for tests.
    private static HostBuilder CreateHostBuilder(string[] args) => new HostBuilder();
}
