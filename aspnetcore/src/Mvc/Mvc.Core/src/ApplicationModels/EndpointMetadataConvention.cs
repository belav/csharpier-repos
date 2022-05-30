// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Mvc.ApplicationModels;

internal sealed class EndpointMetadataConvention : IActionModelConvention
{
    private static readonly MethodInfo PopulateMetadataForEndpointMethod = typeof(EndpointMetadataConvention).GetMethod(nameof(PopulateMetadataForEndpoint), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo PopulateMetadataForParameterMethod = typeof(EndpointMetadataConvention).GetMethod(nameof(PopulateMetadataForParameter), BindingFlags.NonPublic | BindingFlags.Static)!;
    private readonly IServiceProvider _serviceProvider;

    public EndpointMetadataConvention(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Apply(ActionModel action)
    {
        // Get metadata from parameter types
        ApplyParametersMetadata(action);

        // Get metadata from return type
        ApplyReturnTypeMetadata(action);
    }

    private void ApplyReturnTypeMetadata(ActionModel action)
    {
        var returnType = action.ActionMethod.ReturnType;
        if (AwaitableInfo.IsTypeAwaitable(returnType, out var awaitableInfo))
        {
            returnType = awaitableInfo.ResultType;
        }

        if (returnType is not null && typeof(IEndpointMetadataProvider).IsAssignableFrom(returnType))
        {
            object?[]? invokeArgs = null;

            for (var i = 0; i < action.Selectors.Count; i++)
            {
                // Return type implements IEndpointMetadataProvider
                var context = new EndpointMetadataContext(action.ActionMethod, action.Selectors[i].EndpointMetadata, _serviceProvider);
                invokeArgs ??= new object[1];
                invokeArgs[0] = context;
                PopulateMetadataForEndpointMethod.MakeGenericMethod(returnType).Invoke(null, invokeArgs);
            }
        }
    }

    private void ApplyParametersMetadata(ActionModel action)
    {
        object?[]? invokeArgs = null;
        var parameters = action.ActionMethod.GetParameters();

        foreach (var parameter in parameters)
        {
            if (typeof(IEndpointParameterMetadataProvider).IsAssignableFrom(parameter.ParameterType))
            {
                for (var i = 0; i < action.Selectors.Count; i++)
                {
                    // Parameter type implements IEndpointParameterMetadataProvider
                    var context = new EndpointParameterMetadataContext(parameter, action.Selectors[i].EndpointMetadata, _serviceProvider);
                    invokeArgs ??= new object[1];
                    invokeArgs[0] = context;
                    PopulateMetadataForParameterMethod.MakeGenericMethod(parameter.ParameterType).Invoke(null, invokeArgs);
                }
            }

            if (typeof(IEndpointMetadataProvider).IsAssignableFrom(parameter.ParameterType))
            {
                for (var i = 0; i < action.Selectors.Count; i++)
                {
                    // Return type implements IEndpointMetadataProvider
                    var context = new EndpointMetadataContext(action.ActionMethod, action.Selectors[i].EndpointMetadata, _serviceProvider);
                    invokeArgs ??= new object[1];
                    invokeArgs[0] = context;
                    PopulateMetadataForEndpointMethod.MakeGenericMethod(parameter.ParameterType).Invoke(null, invokeArgs);
                }
            }
        }
    }

    private static void PopulateMetadataForParameter<T>(EndpointParameterMetadataContext parameterContext)
        where T : IEndpointParameterMetadataProvider
    {
        T.PopulateMetadata(parameterContext);
    }

    private static void PopulateMetadataForEndpoint<T>(EndpointMetadataContext context)
        where T : IEndpointMetadataProvider
    {
        T.PopulateMetadata(context);
    }
}
