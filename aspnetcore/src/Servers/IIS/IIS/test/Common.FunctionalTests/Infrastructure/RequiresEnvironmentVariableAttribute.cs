using System;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequiresEnvironmentVariableAttribute : Attribute, ITestCondition
{
    private readonly string _name;

    public RequiresEnvironmentVariableAttribute(string name)
    {
        _name = name;
    }

    public bool IsMet => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(_name));

    public string SkipReason => $"Environment variable {_name} is required to run this test.";
}
