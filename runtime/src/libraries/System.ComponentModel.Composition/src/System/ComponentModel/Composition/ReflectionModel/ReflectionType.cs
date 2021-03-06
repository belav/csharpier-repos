// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal sealed class ReflectionType : ReflectionMember
    {
        private readonly Type _type;

        public ReflectionType(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            _type = type;
        }

        public override MemberInfo UnderlyingMember
        {
            get { return _type; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool RequiresInstance
        {
            get { return true; }
        }

        public override Type ReturnType
        {
            get { return _type; }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Type; }
        }

        public override object? GetValue(object? instance)
        {
            return instance;
        }
    }
}
