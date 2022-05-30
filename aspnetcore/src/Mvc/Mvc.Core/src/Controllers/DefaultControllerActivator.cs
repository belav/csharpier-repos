// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.Controllers;

/// <summary>
/// <see cref="IControllerActivator"/> that uses type activation to create controllers.
/// </summary>
internal sealed class DefaultControllerActivator : IControllerActivator
{
    private readonly ITypeActivatorCache _typeActivatorCache;

    /// <summary>
    /// Creates a new <see cref="DefaultControllerActivator"/>.
    /// </summary>
    /// <param name="typeActivatorCache">The <see cref="ITypeActivatorCache"/>.</param>
    public DefaultControllerActivator(ITypeActivatorCache typeActivatorCache)
    {
        if (typeActivatorCache == null)
        {
            throw new ArgumentNullException(nameof(typeActivatorCache));
        }

        _typeActivatorCache = typeActivatorCache;
    }

    /// <inheritdoc />
    public object Create(ControllerContext controllerContext)
    {
        if (controllerContext == null)
        {
            throw new ArgumentNullException(nameof(controllerContext));
        }

        if (controllerContext.ActionDescriptor == null)
        {
            throw new ArgumentException(Resources.FormatPropertyOfTypeCannotBeNull(
                nameof(ControllerContext.ActionDescriptor),
                nameof(ControllerContext)));
        }

        var controllerTypeInfo = controllerContext.ActionDescriptor.ControllerTypeInfo;

        if (controllerTypeInfo == null)
        {
            throw new ArgumentException(Resources.FormatPropertyOfTypeCannotBeNull(
                nameof(controllerContext.ActionDescriptor.ControllerTypeInfo),
                nameof(ControllerContext.ActionDescriptor)));
        }

        var serviceProvider = controllerContext.HttpContext.RequestServices;
        return _typeActivatorCache.CreateInstance<object>(serviceProvider, controllerTypeInfo.AsType());
    }

    /// <inheritdoc />
    public void Release(ControllerContext context, object controller)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        if (controller is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public ValueTask ReleaseAsync(ControllerContext context, object controller)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        if (controller is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable.DisposeAsync();
        }

        Release(context, controller);
        return default;
    }
}
