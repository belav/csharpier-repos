using System.Reflection;

namespace Microsoft.AspNetCore.Analyzers;

public static class TestData
{
    public static string GetMicrosoftNETCoreAppRefPackageVersion() =>
        GetTestDataValue("MicrosoftNETCoreAppRefVersion");

    public static string GetRepoRoot() => GetTestDataValue("RepoRoot");

    private static string GetTestDataValue(string key) =>
        typeof(TestData).Assembly
            .GetCustomAttributes<TestDataAttribute>()
            .Single(d => d.Key == key)
            .Value;
}
