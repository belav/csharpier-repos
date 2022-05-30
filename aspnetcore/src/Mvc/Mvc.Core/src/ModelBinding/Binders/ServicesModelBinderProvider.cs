// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

/// <summary>
/// An <see cref="IModelBinderProvider"/> for binding from the <see cref="IServiceProvider"/>.
/// </summary>
public class ServicesModelBinderProvider : IModelBinderProvider
{
    private readonly ServicesModelBinder _optionalServicesBinder = new() { IsOptional = true };
    private readonly ServicesModelBinder _servicesBinder = new();

    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.BindingInfo.BindingSource != null &&
            context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Services))
        {
            // IsRequired will be false for a Reference Type
            // without a default value in a oblivious nullability context
            // however, for services we shoud treat them as required
            var isRequired = context.Metadata.IsRequired ||
                    (context.Metadata.Identity.ParameterInfo?.HasDefaultValue != true &&
                        !context.Metadata.ModelType.IsValueType &&
                        context.Metadata.NullabilityState == NullabilityState.Unknown);

            return isRequired ? _servicesBinder : _optionalServicesBinder;
        }

        return null;
    }
}
