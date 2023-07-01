using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.Test;

public class EntityFrameworkCoreDataProtectionBuilderExtensionsTests
{
    [Fact]
    public void PersistKeysToEntityFrameworkCore_UsesEntityFrameworkCoreXmlRepository()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddDbContext<DataProtectionKeyContext>()
            .AddDataProtection()
            .PersistKeysToDbContext<DataProtectionKeyContext>();
        var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);
        var keyManagementOptions = serviceProvider.GetRequiredService<
            IOptions<KeyManagementOptions>
        >();
        Assert.IsType<EntityFrameworkCoreXmlRepository<DataProtectionKeyContext>>(
            keyManagementOptions.Value.XmlRepository
        );
    }
}
