using System;

namespace BenchmarkDotNet.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
internal sealed class ParameterizedJobConfigAttribute : AspNetCoreBenchmarkAttribute
{
    public ParameterizedJobConfigAttribute(Type configType)
        : base(configType) { }
}
