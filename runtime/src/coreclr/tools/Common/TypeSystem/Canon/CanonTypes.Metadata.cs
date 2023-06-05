// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

using Internal.NativeFormat;

using Debug = System.Diagnostics.Debug;

namespace Internal.TypeSystem
{
    partial public abstract class CanonBaseType : MetadataType
    {
        protected override MethodImplRecord[] ComputeVirtualMethodImplsForType()
        {
            return Array.Empty<MethodImplRecord>();
        }

        public override MetadataType MetadataBaseType => (MetadataType)BaseType;

        public override DefType[] ExplicitlyImplementedInterfaces => Array.Empty<DefType>();

        public override bool IsAbstract => false;

        public override bool IsBeforeFieldInit => false;

        public override bool IsSequentialLayout => false;

        public override bool IsExplicitLayout => false;

        public override ModuleDesc Module => _context.SystemModule;

        public override bool IsModuleType => false;

        public override MethodImplRecord[] FindMethodsImplWithMatchingDeclName(string name)
        {
            return null;
        }

        public override ClassLayoutMetadata GetClassLayout()
        {
            return default(ClassLayoutMetadata);
        }

        public override MetadataType GetNestedType(string name)
        {
            return null;
        }

        public override IEnumerable<MetadataType> GetNestedTypes()
        {
            return Array.Empty<MetadataType>();
        }

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName)
        {
            return false;
        }
    }

    partial internal sealed class CanonType
    {
        public override bool IsSealed => false;
    }

    partial internal sealed class UniversalCanonType
    {
        public override bool IsSealed => true;
    }
}
