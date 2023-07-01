using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

/// <summary>
/// Describes an handler parameter.
/// </summary>
public class HandlerParameterDescriptor : ParameterDescriptor, IParameterInfoParameterDescriptor
{
    /// <summary>
    /// Gets or sets the <see cref="System.Reflection.ParameterInfo"/>.
    /// </summary>
    public ParameterInfo ParameterInfo { get; set; } = default!;
}
