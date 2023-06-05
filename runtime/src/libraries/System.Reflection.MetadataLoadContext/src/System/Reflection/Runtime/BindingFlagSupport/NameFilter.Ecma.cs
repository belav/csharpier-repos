// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Reflection.TypeLoading.Ecma;

namespace System.Reflection
{
    partial internal abstract class NameFilter
    {
        public abstract bool Matches(StringHandle stringHandle, MetadataReader reader);
    }

    partial internal sealed class NameFilterCaseSensitive : NameFilter
    {
        public sealed override bool Matches(StringHandle stringHandle, MetadataReader reader) =>
            stringHandle.Equals(_expectedNameUtf8, reader);
    }

    partial internal sealed class NameFilterCaseInsensitive : NameFilter
    {
        public sealed override bool Matches(StringHandle stringHandle, MetadataReader reader) =>
            reader.StringComparer.Equals(stringHandle, ExpectedName, true);
    }
}
