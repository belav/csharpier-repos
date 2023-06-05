//------------------------------------------------------------------------------
// <copyright file="RegexRunnerFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

// This RegexRunnerFactory class is a base class for compiled regex code.
// we need to compile a factory because Type.CreateInstance is much slower
// than calling the constructor directly.

namespace System.Text.RegularExpressions
{
    using System.ComponentModel;

    internal
#if !SILVERLIGHT
    /// <internalonly/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public
#endif
#if !SILVERLIGHT
    abstract class RegexRunnerFactory
    {
#else
    abstract class RegexRunnerFactory
    {
#endif
        protected RegexRunnerFactory() { }

        protected internal abstract RegexRunner CreateInstance();
    }
}
