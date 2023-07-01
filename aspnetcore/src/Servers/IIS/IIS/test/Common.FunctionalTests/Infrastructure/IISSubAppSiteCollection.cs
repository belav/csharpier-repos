using Xunit;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[CollectionDefinition(Name)]
public class IISSubAppSiteCollection : ICollectionFixture<IISSubAppSiteFixture>
{
    public const string Name = nameof(IISSubAppSiteCollection);
}
