// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ILLink.Shared.TypeSystemProxy;

// This is needed due to NativeAOT which doesn't enable nullable globally yet
#nullable enable

namespace ILLink.Shared.TrimAnalysis
{
    partial
    // Shared helpers to go from MethodProxy to dataflow values.
    public class FlowAnnotations
    {
        partial internal bool MethodRequiresDataFlowAnalysis(MethodProxy method);

        partial internal MethodReturnValue GetMethodReturnValue(
            MethodProxy method,
            DynamicallyAccessedMemberTypes dynamicallyAccessedMemberTypes
        );

        partial internal MethodReturnValue GetMethodReturnValue(MethodProxy method);

        partial internal GenericParameterValue GetGenericParameterValue(
            GenericParameterProxy genericParameter
        );

        partial internal MethodParameterValue GetMethodThisParameterValue(
            MethodProxy method,
            DynamicallyAccessedMemberTypes dynamicallyAccessedMemberTypes
        );

        partial internal MethodParameterValue GetMethodThisParameterValue(MethodProxy method);

        partial internal MethodParameterValue GetMethodParameterValue(
            ParameterProxy param,
            DynamicallyAccessedMemberTypes dynamicallyAccessedMemberTypes
        );

        partial internal MethodParameterValue GetMethodParameterValue(ParameterProxy param);
    }
}
