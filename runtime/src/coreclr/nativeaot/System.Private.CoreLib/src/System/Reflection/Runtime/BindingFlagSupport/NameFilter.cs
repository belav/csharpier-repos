// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Reflection.Runtime.BindingFlagSupport
{
    partial internal abstract class NameFilter
    {
        protected NameFilter(string expectedName)
        {
            ExpectedName = expectedName;
        }

        public abstract bool Matches(string name);
        protected string ExpectedName { get; }
    }

    partial internal sealed class NameFilterCaseSensitive : NameFilter
    {
        public NameFilterCaseSensitive(string expectedName)
            : base(expectedName) { }

        public sealed override bool Matches(string name) =>
            name.Equals(ExpectedName, StringComparison.Ordinal);
    }

    partial internal sealed class NameFilterCaseInsensitive : NameFilter
    {
        public NameFilterCaseInsensitive(string expectedName)
            : base(expectedName) { }

        public sealed override bool Matches(string name) =>
            name.Equals(ExpectedName, StringComparison.OrdinalIgnoreCase);
    }
}
