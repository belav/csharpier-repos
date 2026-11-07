//------------------------------------------------------------------------------
// <copyright file="MimeParameterReader.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Services.Protocols
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web.Services;
    using System.Xml.Serialization;

    /// <include file='doc\MimeParameterReader.uex' path='docs/doc[@for="MimeParameterReader"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public abstract class MimeParameterReader : MimeFormatter
    {
        /// <include file='doc\MimeParameterReader.uex' path='docs/doc[@for="MimeParameterReader.Read"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public abstract object[] Read(HttpRequest request);
    }
}
