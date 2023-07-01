using System;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[Flags]
public enum IISCapability
{
    None = 0,
    Websockets = 1,
    WindowsAuthentication = 2,
    PoolEnvironmentVariables = 4,
    DynamicCompression = 8,
    ApplicationInitialization = 16,
    TracingModule = 32,
    FailedRequestTracingModule = 64,
    BasicAuthentication = 128
}
