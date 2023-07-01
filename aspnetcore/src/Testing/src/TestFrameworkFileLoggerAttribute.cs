using System;

namespace Microsoft.AspNetCore.Testing;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class TestFrameworkFileLoggerAttribute : TestOutputDirectoryAttribute
{
    public TestFrameworkFileLoggerAttribute(
        string preserveExistingLogsInOutput,
        string tfm,
        string baseDirectory = null
    )
        : base(preserveExistingLogsInOutput, tfm, baseDirectory) { }
}
