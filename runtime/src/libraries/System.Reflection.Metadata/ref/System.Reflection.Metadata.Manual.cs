// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Reflection.Metadata
{
    partial public readonly struct AssemblyDefinition
    {
        public System.Reflection.AssemblyName GetAssemblyName()
        {
            throw null;
        }
    }

    partial public readonly struct AssemblyReference
    {
        public System.Reflection.AssemblyName GetAssemblyName()
        {
            throw null;
        }
    }

    partial public class ImageFormatLimitationException : System.Exception
    {
        protected ImageFormatLimitationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context
        ) { }
    }
}
