using System;

namespace Microsoft.Extensions.Internal;

/// <summary>
/// Marks the constructor to be used when activating type using <see cref="ActivatorUtilities"/>.
/// </summary>

// Do not take a dependency on this class unless you are explicitly trying to avoid taking a
// dependency on Microsoft.AspNetCore.DependencyInjection.Abstractions.
[AttributeUsage(AttributeTargets.All)]
internal sealed class ActivatorUtilitiesConstructorAttribute : Attribute { }
