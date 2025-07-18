// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Reflection;

namespace Moq
{
    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal abstract class ProxyFactory
    After:
        abstract class ProxyFactory
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal abstract class ProxyFactory
    After:
        abstract class ProxyFactory
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal abstract class ProxyFactory
    After:
        abstract class ProxyFactory
    */
    abstract class ProxyFactory
    {
        /// <summary>
        /// Gets the global <see cref="ProxyFactory"/> instance used by Moq.
        /// </summary>
        public static ProxyFactory Instance { get; } = new CastleProxyFactory();

        public abstract object CreateProxy(
            Type mockType,
            IInterceptor interceptor,
            Type[] interfaces,
            object[] arguments
        );

        public abstract bool IsMethodVisible(MethodInfo method, out string messageIfNotVisible);

        public abstract bool IsTypeVisible(Type type);
    }
}
