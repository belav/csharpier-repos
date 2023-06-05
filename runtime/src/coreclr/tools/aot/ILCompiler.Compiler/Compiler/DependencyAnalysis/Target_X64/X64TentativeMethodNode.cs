// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ILCompiler.DependencyAnalysis.X64;

namespace ILCompiler.DependencyAnalysis
{
    partial public class TentativeMethodNode
    {
        protected override void EmitCode(
            NodeFactory factory,
            ref X64Emitter encoder,
            bool relocsOnly
        )
        {
            encoder.EmitJMP(GetTarget(factory));
        }
    }
}
