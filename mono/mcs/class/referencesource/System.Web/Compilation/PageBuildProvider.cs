//------------------------------------------------------------------------------
// <copyright file="PageBuildProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------



namespace System.Web.Compilation
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.IO;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.Util;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web)]
    internal class PageBuildProvider : TemplateControlBuildProvider
    {
        internal override DependencyParser CreateDependencyParser()
        {
            return new PageDependencyParser();
        }

        protected override TemplateParser CreateParser()
        {
            return new PageParser();
        }

        internal override BaseCodeDomTreeGenerator CreateCodeDomTreeGenerator(TemplateParser parser)
        {
            return new PageCodeDomTreeGenerator((PageParser)parser);
        }

        internal override BuildResultNoCompileTemplateControl CreateNoCompileBuildResult()
        {
            return new BuildResultNoCompilePage(Parser.BaseType, Parser);
        }
    }
}
