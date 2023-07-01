using System;

namespace MockHostTypes;

public interface IWebHost
{
    IServiceProvider Services { get; }
}
