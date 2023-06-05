// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ILCompiler.DependencyAnalysis.ARM;

namespace ILCompiler.DependencyAnalysis
{
    partial public class TentativeMethodNode
    {
        protected override void EmitCode(
            NodeFactory factory,
            ref ARMEmitter encoder,
            bool relocsOnly
        )
        {
            encoder.EmitJMP(GetTarget(factory));
        }
    }
}
