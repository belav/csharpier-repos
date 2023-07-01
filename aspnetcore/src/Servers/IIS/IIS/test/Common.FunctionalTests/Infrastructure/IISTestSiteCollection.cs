using Xunit;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

/// <summary>
/// This type just maps collection names to available fixtures
/// </summary>
[CollectionDefinition(Name)]
public class IISTestSiteCollection : ICollectionFixture<IISTestSiteFixture>
{
    public const string Name = nameof(IISTestSiteCollection);
}

[CollectionDefinition(Name)]
public class IISHttpsTestSiteCollection : ICollectionFixture<IISTestSiteFixture>
{
    public const string Name = nameof(IISHttpsTestSiteCollection);
}
