using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Routing;

internal sealed class DefaultEndpointRouteBuilder : IEndpointRouteBuilder
{
    public DefaultEndpointRouteBuilder(IApplicationBuilder applicationBuilder)
    {
        ApplicationBuilder =
            applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
        DataSources = new List<EndpointDataSource>();
    }

    public IApplicationBuilder ApplicationBuilder { get; }

    public IApplicationBuilder CreateApplicationBuilder() => ApplicationBuilder.New();

    public ICollection<EndpointDataSource> DataSources { get; }

    public IServiceProvider ServiceProvider => ApplicationBuilder.ApplicationServices;
}
