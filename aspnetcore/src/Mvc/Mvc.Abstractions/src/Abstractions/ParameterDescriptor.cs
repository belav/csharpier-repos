using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.AspNetCore.Mvc.Abstractions;

/// <summary>
/// Describes a parameter in an action.
/// </summary>
public class ParameterDescriptor
{
    /// <summary>
    /// Gets or sets the parameter name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the type of the parameter.
    /// </summary>
    public Type ParameterType { get; set; } = default!;

    /// <summary>
    /// Gets or sets the <see cref="ModelBinding.BindingInfo"/> for the parameter.
    /// </summary>
    public BindingInfo? BindingInfo { get; set; }
}
