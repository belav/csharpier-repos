// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Composition;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Microsoft.CodeAnalysis.Experiments
{
    internal interface IExperimentationService : IWorkspaceService
    {
        bool IsExperimentEnabled(string experimentName);
    }

    [ExportWorkspaceService(typeof(IExperimentationService)), Shared]
    internal class DefaultExperimentationService : IExperimentationService
    {
        [ImportingConstructor]
        [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public DefaultExperimentationService()
        {
        }

        public bool IsExperimentEnabled(string experimentName) => false;
    }

    internal static class WellKnownExperimentNames
    {
        public const string PartialLoadMode = "Roslyn.PartialLoadMode";
        public const string TypeImportCompletion = "Roslyn.TypeImportCompletion";
        public const string InsertFullMethodCall = "Roslyn.InsertFullMethodCall";
        public const string TargetTypedCompletionFilter = "Roslyn.TargetTypedCompletionFilter";
        public const string OOPServerGC = "Roslyn.OOPServerGC";
        public const string ImportsOnPasteDefaultEnabled = "Roslyn.ImportsOnPasteDefaultEnabled";
        public const string LspTextSyncEnabled = "Roslyn.LspTextSyncEnabled";
        public const string RemoveUnusedReferences = "Roslyn.RemoveUnusedReferences";
        public const string LSPCompletion = "Roslyn.LSP.Completion";
        public const string UnnamedSymbolCompletionDisabled = "Roslyn.UnnamedSymbolCompletionDisabled";
    }
}
