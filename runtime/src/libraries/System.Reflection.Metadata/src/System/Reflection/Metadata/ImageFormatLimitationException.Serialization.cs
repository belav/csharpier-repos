// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

namespace System.Reflection.Metadata
{
    [Serializable]
    partial public class ImageFormatLimitationException : Exception
    {
        protected ImageFormatLimitationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
