// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    partial public static class CompilerMarshalOverride { }

    [AttributeUsageAttribute(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class CppInlineNamespaceAttribute : Attribute
    {
        public CppInlineNamespaceAttribute(string dottedName) { }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Struct)]
    partial public sealed class HasCopySemanticsAttribute : System.Attribute
    {
        public HasCopySemanticsAttribute() { }
    }

    partial public static class IsBoxed { }

    partial public static class IsByValue { }

    partial public static class IsCopyConstructed { }

    partial public static class IsExplicitlyDereferenced { }

    partial public static class IsImplicitlyDereferenced { }

    partial public static class IsJitIntrinsic { }

    partial public static class IsLong { }

    partial public static class IsPinned { }

    partial public static class IsSignUnspecifiedByte { }

    partial public static class IsUdtReturn { }

    [System.AttributeUsageAttribute(System.AttributeTargets.Struct, Inherited = true)]
    partial public sealed class NativeCppClassAttribute : System.Attribute
    {
        public NativeCppClassAttribute() { }
    }

    [System.AttributeUsageAttribute(
        System.AttributeTargets.Class
            | System.AttributeTargets.Enum
            | System.AttributeTargets.Interface
            | System.AttributeTargets.Struct,
        AllowMultiple = true,
        Inherited = false
    )]
    partial public sealed class RequiredAttributeAttribute : System.Attribute
    {
        public RequiredAttributeAttribute(System.Type requiredContract) { }

        public System.Type RequiredContract
        {
            get { throw null; }
        }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Enum)]
    partial public sealed class ScopelessEnumAttribute : System.Attribute
    {
        public ScopelessEnumAttribute() { }
    }
}
