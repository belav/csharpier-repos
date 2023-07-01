using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Validators;

namespace BenchmarkDotNet.Attributes;

internal sealed class DefaultCoreDebugConfig : ManualConfig
{
    public DefaultCoreDebugConfig()
    {
        AddLogger(ConsoleLogger.Default);
        AddValidator(JitOptimizationsValidator.DontFailOnError);

        AddJob(Job.InProcess.WithStrategy(RunStrategy.Throughput));
    }
}
