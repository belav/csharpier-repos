//------------------------------------------------------------------------------
// <copyright file="MimeParameter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Services.Description
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Reflection;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal class MimeParameter
    {
        string name;
        string typeName;
        CodeAttributeDeclarationCollection attrs;

        internal string Name
        {
            get { return name == null ? string.Empty : name; }
            set { name = value; }
        }

        internal string TypeName
        {
            get { return typeName == null ? string.Empty : typeName; }
            set { typeName = value; }
        }

        internal CodeAttributeDeclarationCollection Attributes
        {
            get
            {
                if (attrs == null)
                    attrs = new CodeAttributeDeclarationCollection();
                return attrs;
            }
        }
    }
}
