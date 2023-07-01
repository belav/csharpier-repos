using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer;

internal sealed class EndpointParameterDescriptor
    : ParameterDescriptor,
        IParameterInfoParameterDescriptor
{
    public ParameterInfo ParameterInfo { get; set; } = default!;
}
