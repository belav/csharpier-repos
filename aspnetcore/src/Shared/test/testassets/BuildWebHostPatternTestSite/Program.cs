using MockHostTypes;

namespace BuildWebHostPatternTestSite;

public class Program
{
    static void Main(string[] args) { }

    public static IWebHost BuildWebHost(string[] args) => new WebHost();
}
