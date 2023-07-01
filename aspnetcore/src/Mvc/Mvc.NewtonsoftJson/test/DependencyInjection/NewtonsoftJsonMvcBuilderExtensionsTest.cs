using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public class NewtonsoftJsonMvcBuilderExtensionsTest
{
    [Fact]
    public void AddNewtonsoftJson_ConfiguresOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services
            .AddMvc()
            .AddNewtonsoftJson(
                (options) =>
                {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                }
            );

        // Assert
        Assert.Single(
            services,
            d => d.ServiceType == typeof(IConfigureOptions<MvcNewtonsoftJsonOptions>)
        );
    }
}
