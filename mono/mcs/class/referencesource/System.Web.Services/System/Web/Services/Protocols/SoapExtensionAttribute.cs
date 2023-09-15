//------------------------------------------------------------------------------
// <copyright file="SoapExtensionAttribute.cs" company="Microsoft">
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

    /// <include file='doc\SoapExtensionAttribute.uex' path='docs/doc[@for="SoapExtensionAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public abstract class SoapExtensionAttribute : System.Attribute
    {
        /// <include file='doc\SoapExtensionAttribute.uex' path='docs/doc[@for="SoapExtensionAttribute.ExtensionType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public abstract Type ExtensionType { get; }

        /// <include file='doc\SoapExtensionAttribute.uex' path='docs/doc[@for="SoapExtensionAttribute.Priority"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public abstract int Priority { get; set; }
    }
}
