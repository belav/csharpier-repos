using MockHostTypes;

namespace CreateWebHostBuilderInvalidSignature;

public class Program
{
    static void Main(string[] args) { }

    // Wrong return type
    public static IWebHost CreateWebHostBuilder(string[] args) => new WebHost();
}
