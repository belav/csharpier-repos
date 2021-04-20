// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.Razor.Compilation
{
    /// <summary>
    /// This class is replaced by RazorCompiledItem and will not be used by the runtime.
    /// </summary>
    [Obsolete("This attribute has been superseded by RazorCompiledItem and will not be used by the runtime.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RazorViewAttribute : Attribute
    {
        /// <summary>
        /// This class is replaced by RazorCompiledItem and will not be used by the runtime.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="viewType">The viewtype.</param>
        public RazorViewAttribute(string path, Type viewType)
        {
            Path = path;
            ViewType = viewType;
        }

        /// <summary>
        /// Gets the path of the view.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the view type.
        /// </summary>
        public Type ViewType { get; }
    }
}