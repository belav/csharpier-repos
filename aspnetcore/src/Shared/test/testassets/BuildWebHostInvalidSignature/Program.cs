using MockHostTypes;

namespace BuildWebHostInvalidSignature;

public class Program
{
    static void Main(string[] args) { }

    // Missing string[] args
    public static IWebHost BuildWebHost() => null;
}
