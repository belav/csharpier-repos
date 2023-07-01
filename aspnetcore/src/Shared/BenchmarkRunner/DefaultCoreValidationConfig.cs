using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace BenchmarkDotNet.Attributes;

internal sealed class DefaultCoreValidationConfig : ManualConfig
{
    public DefaultCoreValidationConfig()
    {
        AddLogger(ConsoleLogger.Default);

        AddJob(Job.Dry.WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}
