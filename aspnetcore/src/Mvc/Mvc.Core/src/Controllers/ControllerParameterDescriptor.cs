using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.Controllers;

/// <summary>
/// A descriptor for method parameters of an action method.
/// </summary>
public class ControllerParameterDescriptor : ParameterDescriptor, IParameterInfoParameterDescriptor
{
    /// <summary>
    /// Gets or sets the <see cref="System.Reflection.ParameterInfo"/>.
    /// </summary>
    public ParameterInfo ParameterInfo { get; set; } = default!;
}
