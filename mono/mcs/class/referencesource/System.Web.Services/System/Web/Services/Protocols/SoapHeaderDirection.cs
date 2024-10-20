//------------------------------------------------------------------------------
// <copyright file="SoapHeaderDirection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Services.Protocols
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Web.Services;
    using System.Xml.Serialization;

    /// <include file='doc\SoapHeaderDirection.uex' path='docs/doc[@for="SoapHeaderDirection"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [Flags]
    public enum SoapHeaderDirection
    {
        /// <include file='doc\SoapHeaderDirection.uex' path='docs/doc[@for="SoapHeaderDirection.In"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        In = 0x1,

        /// <include file='doc\SoapHeaderDirection.uex' path='docs/doc[@for="SoapHeaderDirection.Out"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Out = 0x2,

        /// <include file='doc\SoapHeaderDirection.uex' path='docs/doc[@for="SoapHeaderDirection.InOut"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        InOut = 0x3,

        /// <include file='doc\SoapHeaderDirection.uex' path='docs/doc[@for="SoapHeaderDirection.Fault"]/*' />
        Fault = 0x4,
    }
}
