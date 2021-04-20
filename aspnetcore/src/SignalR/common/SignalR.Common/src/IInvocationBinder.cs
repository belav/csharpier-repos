// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Microsoft.AspNetCore.SignalR
{
    /// <summary>
    /// Class used by <see cref="IHubProtocol"/>s to get the <see cref="Type"/>(s) expected by the hub message being deserialized.
    /// </summary>
    public interface IInvocationBinder
    {
        /// <summary>
        /// Gets the <see cref="Type"/> the invocation represented by the <paramref name="invocationId"/> is expected to contain.
        /// </summary>
        /// <param name="invocationId">The ID of the invocation being received.</param>
        /// <returns>The <see cref="Type"/> the invocation is expected to contain.</returns>
        Type GetReturnType(string invocationId);

        /// <summary>
        /// Gets the list of <see cref="Type"/>s the method represented by <paramref name="methodName"/> takes as arguments.
        /// </summary>
        /// <param name="methodName">The name of the method being called.</param>
        /// <returns>A list of <see cref="Type"/>s the method takes as arguments.</returns>
        IReadOnlyList<Type> GetParameterTypes(string methodName);

        /// <summary>
        /// Gets the <see cref="Type"/> the stream item is expected to contain.
        /// </summary>
        /// <param name="streamId">The ID of the stream the stream item is a part of.</param>
        /// <returns>The <see cref="Type"/> of the item the stream contains.</returns>
        Type GetStreamItemType(string streamId);
    }
}
