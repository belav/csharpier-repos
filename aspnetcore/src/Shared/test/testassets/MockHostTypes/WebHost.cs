using System;

namespace MockHostTypes;

public class WebHost : IWebHost
{
    public IServiceProvider Services { get; } = new ServiceProvider();
}
