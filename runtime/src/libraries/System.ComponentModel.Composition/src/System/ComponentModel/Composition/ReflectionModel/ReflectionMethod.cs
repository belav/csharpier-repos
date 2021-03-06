// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal sealed partial class ReflectionMethod : ReflectionMember
    {
        private readonly MethodInfo _method;

        public ReflectionMethod(MethodInfo method)
        {
            ArgumentNullException.ThrowIfNull(method);

            _method = method;
        }

        public MethodInfo UnderlyingMethod
        {
            get { return _method; }
        }

        public override MemberInfo UnderlyingMember
        {
            get { return UnderlyingMethod; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool RequiresInstance
        {
            get { return !UnderlyingMethod.IsStatic; }
        }

        public override Type ReturnType
        {
            get { return UnderlyingMethod.ReturnType; }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Method; }
        }

        public override object GetValue(object? instance)
        {
            return SafeCreateExportedDelegate(instance, _method);
        }

        private static ExportedDelegate SafeCreateExportedDelegate(object? instance, MethodInfo method)
        {
            return new ExportedDelegate(instance, method);
        }
    }
}
