using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Antiforgery;

public class TestOptionsManager : IOptions<AntiforgeryOptions>
{
    public TestOptionsManager() { }

    public TestOptionsManager(AntiforgeryOptions options)
    {
        Value = options;
    }

    public AntiforgeryOptions Value { get; set; } = new AntiforgeryOptions();
}
