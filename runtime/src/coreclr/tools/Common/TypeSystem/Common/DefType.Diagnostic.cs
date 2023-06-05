// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial
    /// <summary>
    /// Type with metadata available that is equivalent to a TypeDef record in an ECMA 335 metadata stream.
    /// A class, an interface, or a value type.
    /// </summary>
    public abstract class DefType
    {
        /// <summary>
        /// Gets the Name of a type. This must not throw
        /// </summary>
        public abstract string DiagnosticName { get; }

        /// <summary>
        /// Gets the Namespace of a type. This must not throw
        /// </summary>
        public abstract string DiagnosticNamespace { get; }
    }
}
