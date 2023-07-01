using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Testing;

public class AspNetTestFramework : XunitTestFramework
{
    public AspNetTestFramework(IMessageSink messageSink)
        : base(messageSink) { }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
        new AspNetTestFrameworkExecutor(
            assemblyName,
            SourceInformationProvider,
            DiagnosticMessageSink
        );
}
