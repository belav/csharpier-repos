//------------------------------------------------------------------------------
// <copyright file="MimeReturnWriter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Services.Protocols
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Web.Services;
    using System.Xml.Serialization;

    internal abstract class MimeReturnWriter : MimeFormatter
    {
        internal abstract void Write(
            HttpResponse response,
            Stream outputStream,
            object returnValue
        );
    }
}
