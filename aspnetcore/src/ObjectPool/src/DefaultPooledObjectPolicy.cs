// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.ObjectPool
{
    /// <summary>
    /// Default implementation for <see cref="PooledObjectPolicy{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object which is being pooled.</typeparam>
    public class DefaultPooledObjectPolicy<T> : PooledObjectPolicy<T> where T : class, new()
    {
        /// <inheritdoc />
        public override T Create()
        {
            return new T();
        }

        /// <inheritdoc />
        public override bool Return(T obj)
        {
            // DefaultObjectPool<T> doesn't call 'Return' for the default policy.
            // So take care adding any logic to this method, as it might require changes elsewhere.
            return true;
        }
    }
}
