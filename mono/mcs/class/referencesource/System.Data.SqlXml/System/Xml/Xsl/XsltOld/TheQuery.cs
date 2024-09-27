//------------------------------------------------------------------------------
// <copyright file="TheQuery.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using MS.Internal.Xml.XPath;
    using Res = System.Xml.Utils.Res;

    internal sealed class TheQuery
    {
        internal InputScopeManager _ScopeManager;
        private CompiledXpathExpr _CompiledQuery;

        internal CompiledXpathExpr CompiledQuery
        {
            get { return _CompiledQuery; }
        }

        internal TheQuery(CompiledXpathExpr compiledQuery, InputScopeManager manager)
        {
            _CompiledQuery = compiledQuery;
            _ScopeManager = manager.Clone();
        }
    }
}
