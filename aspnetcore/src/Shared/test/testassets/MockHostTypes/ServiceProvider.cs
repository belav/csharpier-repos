using System;

namespace MockHostTypes;

public class ServiceProvider : IServiceProvider
{
    public object GetService(Type serviceType) => null;
}
