// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
    public partial class MissingMemberException : MemberAccessException, ISerializable
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string FormatSignature(byte[]? signature);
    }
}
