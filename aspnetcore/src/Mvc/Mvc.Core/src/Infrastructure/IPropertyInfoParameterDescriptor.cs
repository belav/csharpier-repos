using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Microsoft.AspNetCore.Mvc.Infrastructure;

/// <summary>
/// A <see cref="ParameterDescriptor"/> for bound properties.
/// </summary>
public interface IPropertyInfoParameterDescriptor
{
    /// <summary>
    /// Gets the <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    PropertyInfo PropertyInfo { get; }
}
