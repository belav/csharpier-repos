// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;
using System.Runtime.InteropServices;

namespace System.Diagnostics.Metrics
{
    internal interface IObjectSequence
    {
        Span<object?> AsSpan();
    }

    partial internal struct ObjectSequence1 : IEquatable<ObjectSequence1>, IObjectSequence
    {
        public Span<object?> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref Value1, 1);
        }
    }

    partial internal struct ObjectSequence2 : IEquatable<ObjectSequence2>, IObjectSequence
    {
        public Span<object?> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref Value1, 2);
        }

        public override int GetHashCode() => HashCode.Combine(Value1, Value2);
    }

    partial internal struct ObjectSequence3 : IEquatable<ObjectSequence3>, IObjectSequence
    {
        public Span<object?> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref Value1, 3);
        }

        public override int GetHashCode() => HashCode.Combine(Value1, Value2, Value3);
    }

    partial internal struct ObjectSequenceMany : IEquatable<ObjectSequenceMany>, IObjectSequence
    {
        public Span<object?> AsSpan()
        {
            return _values.AsSpan();
        }

        public override int GetHashCode()
        {
            HashCode h = default;
            for (int i = 0; i < _values.Length; i++)
            {
                h.Add(_values[i]);
            }
            return h.ToHashCode();
        }
    }
}
