using System;

namespace MockHostTypes;

public class Host : IHost
{
    public IServiceProvider Services { get; } = new ServiceProvider();
}
