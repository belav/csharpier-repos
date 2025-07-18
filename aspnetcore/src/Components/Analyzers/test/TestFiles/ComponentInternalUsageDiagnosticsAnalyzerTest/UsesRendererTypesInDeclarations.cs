// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components.Analyzers.Tests.TestFiles.ComponentInternalUsageDiagnosticsAnalyzerTest
{
    /*MMBaseClass*/class UsesRendererTypesInDeclarations : Renderer
    {
        private readonly Renderer /*MMField*/
        _field = null;

        public UsesRendererTypesInDeclarations()
            /*MMInvocation*/: base(null, null) { }

        public override Dispatcher Dispatcher => throw new NotImplementedException();

        /*MMProperty*/public Renderer Property { get; set; }

        protected override void HandleException(Exception exception)
        {
            throw new NotImplementedException();
        }

        /*MMParameter*/protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            throw new NotImplementedException();
        }

        /*MMReturnType*/private Renderer GetRenderer() => _field;

        public interface ITestInterface { }
    }
}
