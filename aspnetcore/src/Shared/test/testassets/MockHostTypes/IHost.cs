using System;

namespace MockHostTypes;

public interface IHost
{
    IServiceProvider Services { get; }
}
