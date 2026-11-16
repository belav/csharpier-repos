//------------------------------------------------------------------------------
// <copyright file="DtdParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">helenak</owner>
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Xml;

namespace System.Xml
{
    internal partial interface IDtdParser
    {
        Task<IDtdInfo> ParseInternalDtdAsync(IDtdParserAdapter adapter, bool saveInternalSubset);

        Task<IDtdInfo> ParseFreeFloatingDtdAsync(
            string baseUri,
            string docTypeName,
            string publicId,
            string systemId,
            string internalSubset,
            IDtdParserAdapter adapter
        );
    }
}
