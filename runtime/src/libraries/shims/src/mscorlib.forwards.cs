// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Add any internal types that we need to forward from mscorlib.

// These types are required for Desktop to Core serialization as they are not covered by GenFacades because they are marked as internal.
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.GenericComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.NullableComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ObjectComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.GenericEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.NullableEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ObjectEqualityComparer<>))]
// This is required for back-compatibility with .NET Core 2.0 as we exposed the NonRandomizedStringEqualityComparer inside the serialization blob
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.NonRandomizedStringEqualityComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ByteEqualityComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.EnumEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.ListDictionaryInternal))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.CultureAwareComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.OrdinalComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.UnitySerializationHolder))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Diagnostics.Contracts.ContractException))]
// This is required for back-compatibility with legacy Xamarin which had Stack<T> and Queue<T> in mscorlib
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.Stack<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.Queue<>))]
