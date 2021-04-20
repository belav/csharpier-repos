﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Razor.Language.CodeGeneration
{
    public readonly struct LinePragma : IEquatable<LinePragma>
    {
        public LinePragma(int startLineIndex, int lineCount, string filePath)
        {
            if (startLineIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startLineIndex));
            }

            if (lineCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lineCount));
            }

            StartLineIndex = startLineIndex;
            LineCount = lineCount;
            FilePath = filePath;
        }

        public int StartLineIndex { get; }

        public int EndLineIndex => StartLineIndex + LineCount;

        public int LineCount { get; }

        public string FilePath { get; }

        public override bool Equals(object obj)
        {
            return obj is LinePragma other ? Equals(other) : false;
        }

        public bool Equals(LinePragma other)
        {
            return StartLineIndex == other.StartLineIndex &&
                LineCount == other.LineCount &&
                string.Equals(FilePath, other.FilePath, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            var hash = HashCodeCombiner.Start();
            hash.Add(StartLineIndex);
            hash.Add(LineCount);
            hash.Add(FilePath);
            return hash;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Line index {0}, Count {1} - {2}", StartLineIndex, LineCount, FilePath);
        }
    }
}
