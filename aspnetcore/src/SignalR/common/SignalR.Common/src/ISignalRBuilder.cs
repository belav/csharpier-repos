using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.SignalR;

/// <summary>
/// A builder abstraction for configuring SignalR object instances.
/// </summary>
public interface ISignalRBuilder
{
    /// <summary>
    /// Gets the builder service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
