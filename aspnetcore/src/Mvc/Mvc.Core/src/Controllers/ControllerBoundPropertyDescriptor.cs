using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.Controllers;

/// <summary>
/// A descriptor for model bound properties of a controller.
/// </summary>
public class ControllerBoundPropertyDescriptor
    : ParameterDescriptor,
        IPropertyInfoParameterDescriptor
{
    /// <summary>
    /// Gets or sets the <see cref="System.Reflection.PropertyInfo"/> for this property.
    /// </summary>
    public PropertyInfo PropertyInfo { get; set; } = default!;
}
