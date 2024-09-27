// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ILCompiler.DependencyAnalysis;
using Internal.IL;
using Internal.TypeSystem;
using DependencyList = ILCompiler.DependencyAnalysisFramework.DependencyNodeCore<ILCompiler.DependencyAnalysis.NodeFactory>.DependencyList;

namespace ILCompiler
{
    /// <summary>
    /// This class is responsible for managing stub methods for interop
    /// </summary>
    public sealed class EmptyInteropStubManager : InteropStubManager
    {
        public override PInvokeILProvider CreatePInvokeILProvider()
        {
            return null;
        }

        public override void AddDependenciesDueToMethodCodePresence(
            ref DependencyList dependencies,
            NodeFactory factory,
            MethodDesc method
        ) { }

        public override void AddInterestingInteropConstructedTypeDependencies(
            ref DependencyList dependencies,
            NodeFactory factory,
            TypeDesc type
        ) { }

        public override void AddMarshalAPIsGenericDependencies(
            ref DependencyList dependencies,
            NodeFactory factory,
            MethodDesc method
        ) { }
    }
}
