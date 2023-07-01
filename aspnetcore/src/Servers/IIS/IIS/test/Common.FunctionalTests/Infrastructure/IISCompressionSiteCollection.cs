using Xunit;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[CollectionDefinition(Name)]
public class IISCompressionSiteCollection : ICollectionFixture<IISCompressionSiteFixture>
{
    public const string Name = nameof(IISCompressionSiteCollection);
}
