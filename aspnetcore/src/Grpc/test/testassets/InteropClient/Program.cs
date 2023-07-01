using System;
using System.Reflection;

namespace InteropTestsClient;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Application started.");

        var runtimeVersion =
            typeof(object).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "Unknown";
        Console.WriteLine($"NetCoreAppVersion: {runtimeVersion}");

        InteropClient.Run(args);
    }
}
